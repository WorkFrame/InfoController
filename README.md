# InfoController
Dispatcht Meldungen unter Berücksichtigung ihrer Schweregrade. Verwaltet eine Delegate-Liste, in die sich Viewer eintragen können. Bietet auch Logging-Funktionen.
Siehe die enthaltenen Projekte InfoControllerDemo und Logging.

## Einsatzbereich

  - InfoController gehört, wie auch alle anderen unter **WorkFrame** liegenden Projekte, ursprünglich zum
   Repository **Vishnu** (https://github.com/VishnuHome/Vishnu), kann aber auch eigenständig für andere Apps verwendet werden.

## Voraussetzungen

  - Läuft auf Systemen ab Windows 10.
  - Entwicklung und Umwandlung mit Visual Studio 2022 Version 17.8 oder höher.
  - .Net Runtime ab 8.0.2.

## Schnellstart

Die einzelnen Module (Projekte, Repositories) unterhalb von **WorkFrame** sind teilweise voneinander abhängig,
weshalb folgende Vorgehensweise für die erste Einrichtung empfohlen wird:
  - ### Installation:
	* Ein lokales Basisverzeichnis für alle weiteren WorkFrame-, Vishnu- und sonstigen Hilfs-Verzeichnisse anlegen, zum Beispiel c:\Users\<user>\Documents\MyVishnu.
	* [init.zip](https://github.com/VishnuHome/Setup/raw/master/Vishnu.bin/init.zip) herunterladen und in das Basisverzeichnis entpacken.

	Es entsteht dann folgende Struktur:
	
	![Verzeichnisse nach Installation](./struct.png?raw=true "Verzeichnisstruktur")

## Quellcode und Entwicklung analog zum Repository [Vishnu](https://github.com/VishnuHome/Vishnu)

Für detailliertere Ausführungen sehe bitte dort nach.

## Kurzhinweise

1. Forken des Repositories **InfoController** über den Button Fork
<br/>(Repository https://github.com/WorkFrame/InfoController)

2. Clonen des geforkten Repositories **InfoController** in das existierende Unterverzeichnis
	.../MyVishnu/**WorkFrame**
	
