// Dichiarazione della costante per la porta seriale
const int SERIAL_RATE = 9600;

// Primo potenziometro
const int POTENTIOMETER_PIN = A0;
const int BUTTON_PIN = 8;

//Secondo potenziometro
const int POTENTIOMETER_PIN1 = A5;
const int BUTTON_PIN1 = 2;

// Variabile per memorizzare il valore del potenziometro
int potValue = 0;
int potValue1 = 0;
int buttonValue = 0;
int buttonValue1 = 0;

void setup() {
  // Inizializzazione della comunicazione seriale
  Serial.begin(SERIAL_RATE);
  pinMode(BUTTON_PIN, INPUT_PULLUP);
  pinMode(BUTTON_PIN1, INPUT_PULLUP);
}

void loop() {
  // Leggi il valore del potenziometro
  potValue = analogRead(POTENTIOMETER_PIN);
  potValue1 = analogRead(POTENTIOMETER_PIN1);
  buttonValue = digitalRead(BUTTON_PIN);
  buttonValue1 = digitalRead(BUTTON_PIN1);

  // Invia il valore del potenziometro a Unity attraverso la comunicazione seriale
  Serial.print(potValue);
  Serial.print(";");
  Serial.print(potValue1);
  Serial.print(";");
  Serial.print(buttonValue);
  Serial.print(";");
  Serial.println(buttonValue1);
  Serial.flush();
  }