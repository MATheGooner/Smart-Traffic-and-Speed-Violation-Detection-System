#include <WiFi.h>
#include <HTTPClient.h>
#include <ArduinoJson.h>

// === Traffic Light Pins ===
#define L1_RED 13
#define L1_YELLOW 12
#define L1_GREEN 14

#define L2_RED 27
#define L2_YELLOW 26
#define L2_GREEN 25

#define L3_RED 33
#define L3_YELLOW 32
#define L3_GREEN 15

#define L4_RED 19
#define L4_YELLOW 18
#define L4_GREEN 5

// === Violation Trigger Output Pins (connected to ESP32-CAM input pins) ===
#define L1_VIOLATION_PIN 4
#define L2_VIOLATION_PIN 16
#define L3_VIOLATION_PIN 17
#define L4_VIOLATION_PIN 2

// === Nano Serial Pins ===
#define NANO_A_RX 34
#define NANO_B_RX 35
#define NANO_A_RESET 22
#define NANO_B_RESET 23

HardwareSerial nanoA(1);
HardwareSerial nanoB(2);

// Wi-Fi credentials
const char* ssid = "Timi";
const char* password = "12341234";

// API endpoint
const char* serverName = "https://traffic-control-system-production.up.railway.app/TrafficControlSystem/TrafficDensity/CreateTrafficDensity";

// Store total counts from Nano
int laneCounts[4] = {0, 0, 0, 0};
// Store previous counts for delta computation
int previousCounts[4] = {0, 0, 0, 0};
// Store new cars detected during last period
int deltaCounts[4] = {0, 0, 0, 0};

unsigned long greenDuration = 10000;
unsigned long yellowDuration = 2000;

void setupLights() {
  int pins[] = {
    L1_RED, L1_YELLOW, L1_GREEN,
    L2_RED, L2_YELLOW, L2_GREEN,
    L3_RED, L3_YELLOW, L3_GREEN,
    L4_RED, L4_YELLOW, L4_GREEN
  };
  for (int i = 0; i < 12; i++) pinMode(pins[i], OUTPUT);
}

void setupViolationPins() {
  pinMode(L1_VIOLATION_PIN, OUTPUT);
  pinMode(L2_VIOLATION_PIN, OUTPUT);
  pinMode(L3_VIOLATION_PIN, OUTPUT);
  pinMode(L4_VIOLATION_PIN, OUTPUT);

  digitalWrite(L1_VIOLATION_PIN, LOW);
  digitalWrite(L2_VIOLATION_PIN, LOW);
  digitalWrite(L3_VIOLATION_PIN, LOW);
  digitalWrite(L4_VIOLATION_PIN, LOW);
}

void setLights(int lane, const char* state) {
  int redPins[] = {L1_RED, L2_RED, L3_RED, L4_RED};
  int yellowPins[] = {L1_YELLOW, L2_YELLOW, L3_YELLOW, L4_YELLOW};
  int greenPins[] = {L1_GREEN, L2_GREEN, L3_GREEN, L4_GREEN};

  for (int i = 0; i < 4; i++) {
    digitalWrite(redPins[i], HIGH);
    digitalWrite(yellowPins[i], LOW);
    digitalWrite(greenPins[i], LOW);
  }

  if (lane >= 0 && lane <= 3) {
    if (strcmp(state, "green") == 0) {
      digitalWrite(redPins[lane], LOW);
      digitalWrite(greenPins[lane], HIGH);
    } else if (strcmp(state, "yellow") == 0) {
      digitalWrite(redPins[lane], LOW);
      digitalWrite(yellowPins[lane], HIGH);
    }
  }
}

void triggerViolation(int lane) {
  int pins[] = {
    L1_VIOLATION_PIN,
    L2_VIOLATION_PIN,
    L3_VIOLATION_PIN,
    L4_VIOLATION_PIN
  };

  if (lane >= 0 && lane < 4) {
    Serial.printf("🚨 Triggering violation on Lane %d (GPIO %d)\n", lane + 1, pins[lane]);
    digitalWrite(pins[lane], HIGH);
    delay(500);
    digitalWrite(pins[lane], LOW);
  }
}

String getISO8601Timestamp() {
  struct tm timeinfo;
  if (!getLocalTime(&timeinfo)) return "1970-01-01T00:00:00Z";
  char buffer[30];
  strftime(buffer, sizeof(buffer), "%Y-%m-%dT%H:%M:%SZ", &timeinfo);
  return String(buffer);
}

void sendTrafficData(int laneId, int density, const char* traffic_status, int density_percentage) {
  if (WiFi.status() == WL_CONNECTED) {
    HTTPClient http;
    http.begin(serverName); 
    http.addHeader("Content-Type", "application/json");

    // Create JSON object
    StaticJsonDocument<256> jsonDoc;
    jsonDoc["id"] = 0;
    jsonDoc["laneId"] = laneId + 1; 
    jsonDoc["density"] = density;
    jsonDoc["traffic_status"] = traffic_status;
    jsonDoc["density_percentage"] = density_percentage;
    jsonDoc["date"] = getISO8601Timestamp(); 

    String jsonString;
    serializeJson(jsonDoc, jsonString);

    // Send the HTTP POST request
    int httpResponseCode = http.POST(jsonString);
    Serial.printf("📡 POST to API returned: %d\n", httpResponseCode);

    if (httpResponseCode > 0) {
      String response = http.getString();
      Serial.println("Response:");
      Serial.println(response);
    }

    http.end();
  } else {
    Serial.println("📡 WiFi not connected. Skipping data send.");
  }
}

void cycleTraffic() {
  for (int i = 0; i < 4; i++) {
    deltaCounts[i] = laneCounts[i] - previousCounts[i];
    if (deltaCounts[i] < 0) deltaCounts[i] = 0;
    previousCounts[i] = laneCounts[i];
    sendTrafficData(i, laneCounts[i], "Moderate", map(deltaCounts[i], 0, 20, 0, 100));
  }

  int lanes[4] = {0, 1, 2, 3};
  for (int i = 0; i < 3; i++) {
    for (int j = i + 1; j < 4; j++) {
      if (deltaCounts[lanes[j]] > deltaCounts[lanes[i]]) {
        int temp = lanes[i];
        lanes[i] = lanes[j];
        lanes[j] = temp;
      }
    }
  }

  Serial.println("🔁 Traffic Flow Priority Based on New Cars:");
  for (int i = 0; i < 4; i++) {
    Serial.printf("Lane %d (+%d cars)\n", lanes[i] + 1, deltaCounts[lanes[i]]);
  }

  for (int i = 0; i < 4; i++) {
    int current = lanes[i];
    int next = lanes[(i + 1) % 4];

    Serial.printf("🟢 Lane %d GREEN\n", current + 1);
    setLights(current, "green");
    delay(greenDuration);

    Serial.printf("🟡 Lane %d YELLOW\n", next + 1);
    setLights(next, "yellow");
    delay(yellowDuration);
  }

  setLights(-1, "none");
  Serial.println("♻ Cycle complete.\n");
}

void parseMessage(String msg) {
  msg.trim();
  if (msg.startsWith("COUNT:")) {
    int laneNum = msg.substring(6, 8).substring(1).toInt();
    int count = msg.substring(msg.lastIndexOf(":") + 1).toInt();
    if (laneNum >= 1 && laneNum <= 4) {
      laneCounts[laneNum - 1] = count;
      Serial.printf("✅ Lane %d total count now %d\n", laneNum, count);
    }
  } else if (msg.startsWith("VIOLATION:")) {
    Serial.println("🚨 Violation received");
    if (msg.endsWith("L1")) triggerViolation(0);
    else if (msg.endsWith("L2")) triggerViolation(1);
    else if (msg.endsWith("L3")) triggerViolation(2);
    else if (msg.endsWith("L4")) triggerViolation(3);
  }
}

void loop() {
  static unsigned long lastCycle = 0;
  const unsigned long cycleInterval = 48000;

  if (nanoA.available()) {
    String msg = nanoA.readStringUntil('\n');
    msg.trim();
    Serial.print("[NANO A] Received: ");
    Serial.println(msg);
    parseCountMessage(msg);
  }

  if (nanoB.available()) {
    String msg = nanoB.readStringUntil('\n');
    msg.trim();
    Serial.print("[NANO B] Received: ");
    Serial.println(msg);
    parseCountMessage(msg);
  }

  if (millis() - lastCycle > cycleInterval) {
    cycleTraffic();
    lastCycle = millis();
  }
}
