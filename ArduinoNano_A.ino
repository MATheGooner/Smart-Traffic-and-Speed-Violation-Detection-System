#include <Arduino.h>

// === Pins for Ultrasonic Sensors ===
// Lane 1
const int L1_Count_TRIG = 3;
const int L1_Count_ECHO = 2;
const int L1_Speed_TRIG = 5;
const int L1_Speed_ECHO = 4;

// Lane 2
const int L2_Count_TRIG = 9;
const int L2_Count_ECHO = 8;
const int L2_Speed_TRIG = 11;
const int L2_Speed_ECHO = 10;

// === Constants ===
const float sensorGap = 10.0;     // cm
const float speedLimit = 5;    // m/s

// === State Variables ===
int count_L1 = 0;
int count_L2 = 0;
bool L1_detected = false;
bool L2_detected = false;

void setup() {
  Serial.begin(9600);  // TX → ESP32 GPIO34

  pinMode(L1_Count_TRIG, OUTPUT); pinMode(L1_Count_ECHO, INPUT);
  pinMode(L1_Speed_TRIG, OUTPUT); pinMode(L1_Speed_ECHO, INPUT);
  pinMode(L2_Count_TRIG, OUTPUT); pinMode(L2_Count_ECHO, INPUT);
  pinMode(L2_Speed_TRIG, OUTPUT); pinMode(L2_Speed_ECHO, INPUT);

  Serial.println("Nano A ready for Lane 1 and Lane 2.");
}

float getDistance(int trig, int echo) {
  digitalWrite(trig, LOW); delayMicroseconds(2);
  digitalWrite(trig, HIGH); delayMicroseconds(10);
  digitalWrite(trig, LOW);
  long duration = pulseIn(echo, HIGH, 30000); // 30ms timeout
  return duration * 0.034 / 2.0;
}

void detectSpeed(int trig, int echo, const char* lane) {
  float d = getDistance(trig, echo);
  if (d < 20.0) {
    unsigned long tStart = millis();
    while (getDistance(trig, echo) < 20.0) {
      if (millis() - tStart > 2000) return;
    }
    float timeSec = (millis() - tStart) / 1000.0;
    if (timeSec == 0) return;

    float speed = (sensorGap / 100.0) / timeSec;

    if (speed > speedLimit) {
      Serial.print("VIOLATION:"); Serial.println(lane);
    }
  }
}

void loop() {
  // Lane 1
  float d1 = getDistance(L1_Count_TRIG, L1_Count_ECHO);
  if (d1 < 20.0 && !L1_detected) {
    count_L1++;
    L1_detected = true;
    Serial.print("COUNT:L1:"); Serial.println(count_L1);
  }
  if (d1 >= 20.0) L1_detected = false;

  detectSpeed(L1_Speed_TRIG, L1_Speed_ECHO, "L1");

  // Lane 2
  float d2 = getDistance(L2_Count_TRIG, L2_Count_ECHO);
  if (d2 < 20.0 && !L2_detected) {
    count_L2++;
    L2_detected = true;
    Serial.print("COUNT:L2:"); Serial.println(count_L2);
  }
  if (d2 >= 20.0) L2_detected = false;

  detectSpeed(L2_Speed_TRIG, L2_Speed_ECHO, "L2");

  delay(100);  // debounce
}
