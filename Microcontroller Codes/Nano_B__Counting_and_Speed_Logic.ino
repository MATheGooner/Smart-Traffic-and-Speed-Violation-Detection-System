#include <Arduino.h>

// === Pins for Ultrasonic Sensors ===
// Lane 3
const int L3_Count_TRIG = 3;
const int L3_Count_ECHO = 2;
const int L3_Speed_TRIG = 5;
const int L3_Speed_ECHO = 4;

// Lane 4
const int L4_Count_TRIG = 7;
const int L4_Count_ECHO = 6;
const int L4_Speed_TRIG = 8;
const int L4_Speed_ECHO = 9;

// === Constants ===
const float sensorGap = 10.0;     // cm
const float speedLimit = 0.2;     // m/s

// === State Variables ===
int count_L3 = 0;
int count_L4 = 0;
bool L3_detected = false;
bool L4_detected = false;

void setup() {
  Serial.begin(9600);  // TX → ESP32 GPIO34

  pinMode(L3_Count_TRIG, OUTPUT); pinMode(L3_Count_ECHO, INPUT);
  pinMode(L3_Speed_TRIG, OUTPUT); pinMode(L3_Speed_ECHO, INPUT);
  pinMode(L4_Count_TRIG, OUTPUT); pinMode(L4_Count_ECHO, INPUT);
  pinMode(L4_Speed_TRIG, OUTPUT); pinMode(L4_Speed_ECHO, INPUT);

  Serial.println("Nano B ready for Lane 3 and Lane 4.");
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
  // Lane 3
  float d3 = getDistance(L3_Count_TRIG, L3_Count_ECHO);
  if (d3 < 20.0 && !L3_detected) {
    count_L3++;
    L3_detected = true;
    Serial.print("COUNT:L3:"); Serial.println(count_L3);
  }
  if (d3 >= 20.0) L3_detected = false;

  detectSpeed(L3_Speed_TRIG, L3_Speed_ECHO, "L3");

  // Lane 4
  float d4 = getDistance(L4_Count_TRIG, L4_Count_ECHO);
  if (d4 < 20.0 && !L4_detected) {
    count_L4++;
    L4_detected = true;
    Serial.print("COUNT:L4:"); Serial.println(count_L4);
  }
  if (d4 >= 20.0) L4_detected = false;

  detectSpeed(L4_Speed_TRIG, L4_Speed_ECHO, "L4");

  delay(100);  // debounce
}