// Dichiarazione della costante per la porta seriale
const int SERIAL_RATE = 9600;

// Dichiarazione del pin del potenziometro
const int POTENTIOMETER_PIN = A0;
const int POTENTIOMETER_PIN1 = A1;

// Variabile per memorizzare il valore del potenziometro
int potValue = 0;
int potValue1 = 0;

void setup() {
  // Inizializzazione della comunicazione seriale
  Serial.begin(SERIAL_RATE);
}

void loop() {
  // Leggi il valore del potenziometro
  potValue = analogRead(POTENTIOMETER_PIN);
  potValue1 = analogRead(POTENTIOMETER_PIN1);

  // Invia il valore del potenziometro a Unity attraverso la comunicazione seriale
  Serial.print(potValue);
  Serial.print(";");
  Serial.println(potValue1);
  //Serial.flush();
}