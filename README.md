# Jugend Forscht Projekt: Sensitives Schachbrett zum Lokalisieren der Figuren inklusive Computersoftware
Aus dem Wettbewerb Hessen und Hessen-Mitte in der Kategorie _Technik_

## Allgemeines
Zur einfachen Bearbeitung sind in diesem Repo vollständige Projektmappen hochgeladen.\
__Das Projekt wurde im Mai 2025 abgeschlossen__\
Das heißt, dass an diesem Projekt keine Arbeit mehr folgen wird. Fehler im ChessCORE gerne per Discord an mich und vielleicht werde ich mich nochmal darum kümmern <3  
Weitere Dokumentationen sind ggf. in den entprechenden Branches.

### Struktur der Branches
Die Branches sind unterteilt in die verschiedenen Teile des Projekts
* main: Die neuste Version des Computerverarbeitungsprogramms wird hier abgelegt
* Arduino: In den entsprechenden Ordnern liegen die dot ino Dateien dieser Version.
* ChessCORE: Veralteter Branch für neue Versionen des Computerprogramms
* ChessCORE-Full: Veralteter Branch für die Vollversion des Prototyps 1
* Datenblätter: Wichtige Datenblätter essenzieller Teile, Erklärunterlagen und Forschungsarbeit
* SchachLite: Reduzierte Version des Computer Programms (Obsolet)
* ChessMagnet: Sehr alte grafische Version. Unfertig, aber trotzdem hochgeladen für Vollständigkeit

## ChessCORE Programm
Der Nachfolger von ChessMagnet (Name ist selbsterklärend)\
Das Computerprogramm ist im C# .NET Framework 8.0 von Microsoft als Konsolenanwendung umgesetzt, um plattformübergreifend und mit wenig Ressourcenaufwand zu arbeiten. \
Das CORE in ChessCORE bedeutet, dass das Programm lediglich Core-Features enthält, die für Verarbeitung notwendig sind und ursprünglich auf .NET CORE basieren sollte.

## Klassen (Module)
* Init (in Program.cs): Start Datei und Darstellung des Menüs
* board_visual: Start des Renderers und Figurenverarbeitung
* scom2: Stellt Methoden zur einfachen Kommunikation mit dem Arduino und stellt die Verbindung her (Version 2)
* Renderer: Interpretiert die Werte der database in ein grafisches Schachbrett und bietet grafisches Debugging an
* Storage: Dateien, Snaps und Logs verwalten
* Database.Physical: Speichert alle vom Arduino empfangenen Daten und Kalibrierungswerte
* Database.Display: Speichert alle für die Darstellung vorbereiteten Werte
* ArrayTools: Methoden, um einfache Vergleiche mit 2D Arrays durchführen zu können
Abgelöste Klassen:
* scom: Stellt Methoden zur einfachen Kommunikation mit dem Arduino und stellt die Verbindung her
* Bash: Kommandos an die Linux Konsole versenden

Ich schreibe vielleicht mal irgendwann hier weiter lol

Liebe Grüße
Arthur
