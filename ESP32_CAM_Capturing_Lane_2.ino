#include "esp_camera.h"
#include "FS.h"
#include "SD_MMC.h"
#include <WiFi.h>
#include <HTTPClient.h>
#include <Base64.h>
#include <time.h>

#define TRIGGER_PIN 3         
#define LANE_ID 2              

const char* ssid = "Timi";
const char* password = "12341234";

const char* serverURL = "https://traffic-control-system-production.up.railway.app/TrafficControlSystem/Violation/CreateViolation";

void startCamera() {
  camera_config_t config;
  config.ledc_channel = LEDC_CHANNEL_0;
  config.ledc_timer = LEDC_TIMER_0;
  config.pin_d0 = 5;
  config.pin_d1 = 18;
  config.pin_d2 = 19;
  config.pin_d3 = 21;
  config.pin_d4 = 36;
  config.pin_d5 = 39;
  config.pin_d6 = 34;
  config.pin_d7 = 35;
  config.pin_xclk = 0;
  config.pin_pclk = 22;
  config.pin_vsync = 25;
  config.pin_href = 23;
  config.pin_sscb_sda = 26;
  config.pin_sscb_scl = 27;
  config.pin_pwdn = 32;
  config.pin_reset = -1;
  config.xclk_freq_hz = 20000000;
  config.pixel_format = PIXFORMAT_JPEG;

  if (psramFound()) {
    config.frame_size = FRAMESIZE_VGA;
    config.jpeg_quality = 10;
    config.fb_count = 2;
  } else {
    config.frame_size = FRAMESIZE_QVGA;
    config.jpeg_quality = 12;
    config.fb_count = 1;
  }

  if (esp_camera_init(&config) != ESP_OK) {
    Serial.println("❌ Camera init failed");
    while (1);
  }
}

bool sendImageToServer(camera_fb_t *fb) {
  // Get current time in ISO 8601 format
  time_t now;
  struct tm timeinfo;
  char timeStr[30];

  time(&now);
  localtime_r(&now, &timeinfo);
  strftime(timeStr, sizeof(timeStr), "%Y-%m-%dT%H:%M:%SZ", &timeinfo);  // UTC format

  // Encode image to base64
  String base64Image = base64::encode(fb->buf, fb->len);

  // Define placeholders for vehicleNumber and speed
  String vehicleNumber = "UNKNOWN"; 
  int speed = 0;                     

  // Build JSON payload
  String payload = "{";
  payload += "\"laneId\":" + String(LANE_ID) + ",";
  payload += "\"vehicleNumber\":\"" + vehicleNumber + "\",";
  payload += "\"violationTime\":\"" + String(timeStr) + "\",";
  payload += "\"image\":\"" + base64Image + "\",";
  payload += "\"speed\":" + String(speed);
  payload += "}";

  // Send HTTP POST request
  HTTPClient http;
  http.begin(serverURL);
  http.addHeader("Content-Type", "application/json");

  int httpResponseCode = http.POST(payload);

  if (httpResponseCode > 0) {
    Serial.print("✅ Upload Success. Response code: ");
    Serial.println(httpResponseCode);
  } else {
    Serial.print("❌ Upload failed. Error code: ");
    Serial.println(httpResponseCode);
    Serial.println("❌ Payload: " + payload);
  }

  http.end();
  return httpResponseCode == 200;
}

void captureAndSendImages(String laneLabel, int count) {
  camera_fb_t* frames[count];
 
  for (int i = 0; i < count; i++) {
    frames[i] = esp_camera_fb_get();
    if (!frames[i]) {
      Serial.println("❌ Camera capture failed");
    } else {
      Serial.printf("📸 Captured image %d\n", i + 1);
    }
    delay(150);
  }

  for (int i = 0; i < count; i++) {
    if (!frames[i]) continue;

    sendImageToServer(frames[i]);

    String path = "/" + laneLabel + "_violation_" + String(millis()) + "_" + String(i + 1) + ".jpg";
    File file = SD_MMC.open(path.c_str(), FILE_WRITE);
    if (file) {
      file.write(frames[i]->buf, frames[i]->len);
      Serial.print("✅ Image saved: ");
      Serial.println(path);
      file.close();
    } else {
      Serial.println("❌ File open failed");
    }

    esp_camera_fb_return(frames[i]);
    delay(200); 
  }
}

void setup() {
  Serial.begin(115200);
  pinMode(TRIGGER_PIN, INPUT);

  WiFi.begin(ssid, password);
  Serial.print("🔌 Connecting to Wi-Fi");
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("\n✅ Connected to Wi-Fi");

  // Set up NTP to sync time
  configTime(0, 0, "pool.ntp.org");
  Serial.println("⏱️ Waiting for time sync...");
  struct tm timeinfo;
  while (!getLocalTime(&timeinfo)) {
    Serial.print(".");
    delay(500);
  }
  Serial.println("\n✅ Time synced");

  startCamera();

  if (!SD_MMC.begin()) {
    Serial.println("❌ SD Card Mount Failed");
    while (1);
  }

  if (SD_MMC.cardType() == CARD_NONE) {
    Serial.println("❌ No SD card detected");
    while (1);
  }

  Serial.println("📸 ESP32-CAM Ready. Waiting for rising edge on GPIO3...");
}

void loop() {
  static bool lastState = LOW;
  bool currentState = digitalRead(TRIGGER_PIN);
  if (lastState == LOW && currentState == HIGH) {
    Serial.println("🚨 Rising edge detected — capturing and sending 2 images...");
    captureAndSendImages("L" + String(LANE_ID), 2);
  }

  lastState = currentState;
  delay(30);
}
