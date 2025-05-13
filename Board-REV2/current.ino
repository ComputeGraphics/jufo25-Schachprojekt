

// PA0-7 Pluspol
// PC0-7 Minuspol

//DDRA - Data Direction A
//PORTA - Port On/Off A
//PINA - Pin lesen A


int baud = 9600;
bool self_test = true;
bool interpolate = false;
bool rb = true;
String errors = "";


void setup() {
  // put your setup code here, to run once:
  Serial.begin(baud);
  Serial.setTimeout(32);

  errors = "";
  Serial.println("SYSTEM START - ARDUINO MEGA 2560");


  ADCSRA = (1 << ADEN)                                    // ADC einschalten
           | (1 << ADPS2) | (1 << ADPS1) | (1 << ADPS0);  // Prescaler = 128 (125 kHz bei 16 MHz)

  DDRA = 0b11111111;
  DDRC = 0b11111111;
  DDRF = 0b00000000;
}

void loop() {
  if (Serial.available() > 0) {
    String r = Serial.readStringUntil('\n');
    String rLower = r;
    rLower.toLowerCase();
    if (rb) Serial.println("RB:" + r);
    if (rLower.startsWith("qget")) {
      Serial.println(qget(r.substring(4), false, interpolate));
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
      Serial.println("SYSTEM INFO:\r\nBAUD: " + String(baud) + "\r\nVERSION REV1\r\nJuFo Schach\r\nSelf Test Result:");
      Serial.println(errors);
    } else if (rLower.startsWith("result")) {
      Serial.println(errors);
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
  byte n = buff.charAt(0) - '0';
  byte p = buff.charAt(1) - '0';
  PORTA = (1 << p);
  PORTC = !(1 << n);
  while (loop && String("X") != Serial.readStringUntil('\n')) Serial.println(ADCRead(n));

  if (!loop && corrector) {
    int val0 = ADCRead(n);
    int val1 = ADCRead(n);
    int val2 = ADCRead(n);

    int minVal = min( val0, min(val1, val2) );
    int maxVal = max( val0, max(val1, val2) );

    // Prüfe, ob die Differenz zu groß ist
    if (maxVal - minVal > 20) return -1;

    return ((val0 + val1 + val2) / 3);
  }

  return (ADCRead(n));
}

void qgetRange(String request, bool loop) {
  String buff = request;
  buff.trim();
  byte n = buff.charAt(0) - '0';
  bool tloop = true;

  PORTC = (1 << n);

  //Serial.println("After MP");
  while (tloop && String("X") != Serial.readStringUntil('\n')) {
    for (int i = 7; i >= 0; i--) {
      //writeQ(i);
      PORTA = !(1 << i);

      if (interpolate) {
        int val0 = ADCRead(n);
        int val1 = ADCRead(n);
        int val2 = ADCRead(n);

        int minVal = min( val0, min(val1, val2) );
        int maxVal = max( val0, max(val1, val2) );

        // Prüfe, ob die Differenz zu groß ist
        if (maxVal - minVal > 20) {
          Serial.println(-1);
          return;
        }

        Serial.println((val0 + val1 + val2) / 3);
      }
      else {
        Serial.println(ADCRead(n));
      }
    }
    tloop = loop;
  }
  Serial.flush();
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


