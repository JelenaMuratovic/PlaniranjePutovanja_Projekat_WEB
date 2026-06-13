# Informacioni sistem za planiranje putovanja

Ovaj projekat predstavlja savremenu distribuiranu veb aplikaciju namenjenu sveobuhvatnoj organizaciji i planiranju putovanja. Sistem omogućava korisnicima upravljanje osnovnim podacima o putovanjima, definisanje destinacija, hronološko strukturiranje aktivnosti, sistematsko praćenje troškova i budžeta u realnom vremenu, upravljanje listama za pakovanje, kao i bezbedno deljenje planova sa drugim korisnicima.

Sistem je realizovan u sklopu predmeta **Primena veb programiranja u infrastrukturnim sistemima**.

---

## 🏗️ Arhitektura sistema i komponente

Aplikacija je projektovana po principima mikroservisne arhitekture unutar **Microsoft Service Fabric** platforme, kombinujući stateless i stateful komponente sa eksternom perzistencijom podataka.

### 1. Backend mikroservisi (Service Fabric)

- **`PlaniranjePutovanja.APIGateway` (Stateless):** Centralna ulazna tačka za sve klijentske zahteve. Gateway obavlja rutiranje, agregaciju mikroservisnih odgovora i primenu autorizacionih polisa (`CanViewTravel`, `CanEditTravel`). Izložen je na portu `8127`.
- **`PlaniranjePutovanja.AuthService` (Stateless):** Upravlja registracijom, prijavom korisnika i validacijom JWT tokena. Sadrži administrativne rute za pregled i uklanjanje korisničkih naloga sa sistema. Lozinke se bezbedno heširaju pre upisa u bazu.
- **`PlaniranjePutovanja.TravelService` (Stateless/Stateful):** Obuhvata jezgro biznis logike za upravljanje entitetima putovanja, pripadajućim destinacijama, pojedinačnim aktivnostima i listama obaveza (`Checklists`). Implementira kaskadno brisanje povezanih podataka.
- **`PlaniranjePutovanja.ExpenseService` (Stateful):** Odgovoran za finansijski aspekt putovanja. Automatski kalkuliše ukupan iznos evidentiranih troškova i preostali budžet. Stanje se radi visokih performansi kešira unutar Service Fabric _Reliable Collections_ struktura podataka.
- **`PlaniranjePutovanja.UtilService` (Stateless):** Pruža infrastrukturne uslužne funkcije poput generisanja PDF izveštaja plana putovanja i kreiranja QR kodova/tokena za bezbedno deljenje planova sa nivoima pristupa za pregled ili uređivanje.
- **`PlaniranjePutovanja.Common`:** Zajednička biblioteka koja sadrži deljene interfejse, DTO modele i ugovore za komunikaciju preko Service Fabric Remoting protokola.

### 2. Frontend aplikacija (React)

Klijentski deo aplikacije je razvijen upotrebom **Vite** razvojnog okruženja.

- **Upravljanje stanjem:** Koristi se izvorni **React Context API** kroz namenske kontekstualne prostore (kao što je `useAuth`), čime se izbegava upotreba teških eksternih biblioteka poput Redux-a i obezbeđuje čist, nativni mehanizam za globalno deljenje stanja autentifikacije i korisničkih prava kroz stablo komponenti.
- **Arhitektura koda:** Aplikacija je podeljena na višekratne komponente. Svi HTTP pozivi su strogo izdvojeni u servisne module koji se injektuju u komponente, prateći striktno razdvajanje logike od prezentacionog sloja.

---

## 🚀 Uputstvo za pokretanje sistema

### Preduslovi

Za uspešno podizanje i rad sistema, na lokalnoj mašini je potrebno obezbediti sledeće alate:

- Microsoft Service Fabric SDK & Runtime
- .NET 8.0 SDK
- Microsoft SQL Server (ili lokalna instanca SQLEXPRESS)
- Node.js (LTS verzija)

---

### 1. Konfiguracija i pokretanje baze podataka

Perzistencija podataka se oslanja na SQL Server uz korišćenje Entity Framework Core alata i eksplicitno definisanih SQL migracija.

1. Otvoriti rešenje `PlaniranjePutovanja.sln` unutar Visual Studio razvojnog okruženja sa administratorskim privilegijama.
2. Otvoriti _Package Manager Console_.
3. S obzirom na to da više mikroservisa poseduje sopstvene kontekste baze podataka (`DbContext`) i nezavisne migracije, potrebno je primeniti komandu ažuriranja pojedinačno za svaki projekat koji vrši perzistenciju. Izvršiti sledeće komande:

```bash
Update-Database -Project PlaniranjePutovanja.TravelService
Update-Database -Project PlaniranjePutovanja.AuthService
Update-Database -Project PlaniranjePutovanja.ExpenseService

```

---

### 2. Pokretanje backend servisa

1. Pokrenuti lokalni Service Fabric klaster pomoću alata _Local Cluster Manager_ iz sistemske trake (preporučuje se konfiguracija sa jednim ili pet čvorova).
2. Unutar Visual Studio razvojnog okruženja postaviti aplikativni projekat `PlaniranjePutovanja` kao polazni (_Startup Project_).
3. Pokrenuti izvršavanje pritiskom na taster **F5** ili klikom na opciju **Start**. Razvojno okruženje će automatski kompajlirati sve mikroservise, izvršiti njihovo pakovanje i registrovati ih unutar lokalnog Service Fabric runtime-a.

### 3. Konfiguracija i pokretanje klijentskog dela (Frontend)

1. Otvoriti terminal i pozicionirati se u korenski direktorijum korisničkog interfejsa:

```bash
   cd frontend

```

2. Na osnovu priloženog šablona env.example koji se nalazi u projektu, kreirati lokalnu datoteku pod nazivom .env u korenskom direktorijumu frontend foldera.
3. Unutar kreirane .env datoteke definisati adresu lokalno pokrenutog API Gateway-a prema sledećem uzorku:

```bash
VITE_API_URL=http://localhost:8127/

```

4. Izvršiti instalaciju svih neophodnih biblioteka i zavisnosti definisanih u klijentskoj konfiguraciji:

```bash
npm install

```

5. Pokrenuti klijentsku aplikaciju u lokalnom razvojnom režimu:

```bash
npm run dev
```

6. Nakon uspešnog podizanja razvojnog servera, aplikaciji se može pristupiti putem veb pretraživača na adresi ispisanoj u terminalu (standardno `http://localhost:5173`)

---

## Validacija podataka i bezbednosni kriterijumi

1. Zaštita i autorizacija ruta: Pristup zaštićenim resursima i izvršavanje akcija uslovljeno je validacijom elektronskog potpisa i roka trajanja priloženog JWT tokena unutar HTTP zaglavlja.
2. Validacija biznis logike: Unutar servisa su implementirane striktne poslovne provere podataka pre perzistencije (npr. krajnji datum putovanja ne može prethoditi početnom datumu, dok finansijski budžet ne može poprimiti negativne vrednosti).
3. Konzistentnost i integritet: Uklanjanjem krovnih entiteta sistema, kroz definisane kaskadne mehanizme na nivou baze podataka i poslovne logike, automatski se brišu svi zavisni podređeni elementi (destinacije, aktivnosti, troškovi i check-liste), sprečavajući pojavu nekonzistentnih zapisa u bazi podataka.
