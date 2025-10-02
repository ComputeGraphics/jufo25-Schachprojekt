

// PA0-7 Pluspol
// PC0-7 Minuspol

//DDRA - Data Direction A
//PORTA - Port On/Off A
//PINA - Pin lesen A


int baud = 9600;
bool self_test = true;
bool interpolate = false;
bool rb = false;
String errors = "";


void setup() {
  // put your setup code here, to run once:
  Serial.begin(baud);
  Serial.setTimeout(64);

  errors = "";

  ADCSRA = (1 << ADEN)                                    // ADC einschalten
           | (1 << ADPS2) | (1 << ADPS1) | (1 << ADPS0);  // Prescaler = 128 (125 kHz bei 16 MHz)

  DDRA = 0xFF;
  DDRC = 0xFF;
  DDRF = 0b00000000;
  //Serial.println(String(PORTA, BIN));
  digitalWrite(22, HIGH);
  //Serial.println(String(PORTA, BIN));
  Serial.println("READY");
}

void loop() {
  if (Serial.available() > 0) {
    String r = Serial.readStringUntil('\n');
    String rLower = r;
    rLower.toLowerCase();
    if (rb) Serial.println("RB:" + r);
    if (rLower.startsWith("qget")) {
      qget(r.substring(4), false, interpolate);
    } else if (rLower.startsWith("qstream")) {
      qget(r.substring(7), true, interpolate);
    } else if (rLower.startsWith("qrange")) {
      qgetRange(r.substring(6), false);
    } else if (rLower.startsWith("qsrange")) {
      qgetRange(r.substring(7), true);
    } else if (rLower.startsWith("qall")) {
      qgetall(false);
    } else if (rLower.startsWith("qsall")) {
      qgetall(true);
    } else if (rLower.startsWith("info")) {
      Serial.println("SYSTEM INFO:\r\nBAUD: " + String(baud) + "\r\nVERSION REV2\r\nJuFo Schach\r\nSelf Test Result:");
      Serial.println(errors);
    } else if (rLower.startsWith("result")) {
      if(errors == "") Serial.println("na");
      else Serial.println(errors);
      Serial.flush();
    } else if (rLower.startsWith("test")) {
      setup();
    } else if (rLower.startsWith("ip")) {
      interpolate = !interpolate;
    } else if (rLower.startsWith("rb")) {
      rb = !rb;
    }
  }
}

bool convertBool(char number) {
  if (number == '1') return true;
  else return false;
}

uint16_t ADCRead(uint8_t pin) {
  ADMUX = (0 << REFS1) | (1 << REFS0)  // AVcc als Referenz (5V)
          | (pin & 0x07);              // Pin auswählen (z. B. A0 → MUX0=0)

  ADCSRA |= (1 << ADSC);  // Starte Konvertierung
  while (ADCSRA & (1 << ADSC))
    ;  // Warte bis fertig

  return ADC;  // 10-Bit-Ergebnis (ADCL + ADCH)
}


int qget(String request, bool loop, bool corrector) {
  String buff = request;
  buff.trim();
  if(buff.length() == 1) buff = "0" + buff;
  if (buff.length() == 2) {
    bool tloop = true;

    byte n = buff.charAt(0) - '0';
    byte p = buff.charAt(1) - '0';

    String strg_n = String(n, BIN);
    String strg_p = String(p, BIN);
    while (strg_n.length() < 3) strg_n = "0" + strg_n;
    while (strg_p.length() < 3) strg_p = "0" + strg_p;

    digitalWrite(26, convertBool(strg_n.charAt(2)));
    digitalWrite(27, convertBool(strg_n.charAt(1)));
    digitalWrite(28, convertBool(strg_n.charAt(0)));

    digitalWrite(22, convertBool(strg_p.charAt(0)));
    digitalWrite(23, convertBool(strg_p.charAt(1)));
    digitalWrite(24, convertBool(strg_p.charAt(2)));


    while (tloop && String("X") != Serial.readStringUntil('\n')) {
      if (corrector) {
        int val0 = ADCRead(n);
        int val1 = ADCRead(n);
        int val2 = ADCRead(n);

        int minVal = min(val0, min(val1, val2));
        int maxVal = max(val0, max(val1, val2));

        // Prüfe, ob die Differenz zu groß ist
        if (maxVal - minVal > 20) Serial.println(-1);

        Serial.println((val0 + val1 + val2) / 3);
      } else Serial.println(ADCRead(n));
      tloop = loop;
    }
    uint16_t adc = ADCRead(n);
    PORTA = 0;
    PORTC = 0;
    return (adc);
  } else return -2;
}

void qgetRange(String request, bool loop) {
  String buff = request;
  buff.trim();

  byte n = buff.charAt(0) - '0';
  String strg_n = String(n, BIN);
  while (strg_n.length() < 3) strg_n = "0" + strg_n;

  digitalWrite(26, convertBool(strg_n.charAt(2)));
  digitalWrite(27, convertBool(strg_n.charAt(1)));
  digitalWrite(28, convertBool(strg_n.charAt(0)));

  bool tloop = true;
  while (tloop && String("X") != Serial.readStringUntil('\n')) {
    for (int i = 7; i >= 0; i--) {
      String assigned = String(i, BIN);
      while (assigned.length() < 3) assigned = "0" + assigned;
      digitalWrite(22, convertBool(assigned.charAt(0)));
      digitalWrite(23, convertBool(assigned.charAt(1)));
      digitalWrite(24, convertBool(assigned.charAt(2)));
      delay(50);
      Serial.println(ADCRead(n));
    }
    tloop = loop;
  }
  PORTA = 0;
  PORTC = 0;
}

void qgetall(bool loop) {
  bool tloop = true;
  while (tloop && String("X") != Serial.readStringUntil('\n')) {
    for (int i = 0; i < 8; i++) {
      qgetRange(String(i * 10), false);
    }
    tloop = loop;
  }
}
