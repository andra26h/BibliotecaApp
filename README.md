# BibliotecaApp

## Bibliotecă virtuală

Aplicația a fost construită folosind ASP.NET MVC.

### Diagrama bazei de date

Structura bazei de date permite gestionarea eficientă a informațiilor despre cărți, utilizatori, autori/categorii, recenzii și împrumuturi.

### Schema funcționării aplicației

---

## Tabelul Users

Acest tabel păstrează informațiile esențiale despre utilizatori:

- **IdUser** (cheie primară): Identificator unic pentru utilizator.
- **Email**: Adresa de email a utilizatorului.
- **Password**: Parola criptată a utilizatorului.
- **Username**: Numele de utilizator ales.
- **Role**: Rolul utilizatorului.
- **FirstName/LastName**: Numele și prenumele utilizatorului.

---

## Tabelul Books

Stochează informațiile despre cărțile disponibile:

- **BookId** (cheie primară): Identificator unic pentru fiecare carte.
- **AuthorId** (cheie străină): Referință către autorul care a scris cartea.
- **CategoryId** (cheie străină): Referință către categoria din care face parte.
- **Title**: Titlul cărții.
- **Description**: Scurtă descriere a cărții.
- **AvailableCopies**: Numărul de exemplare disponibile.
- **IsPopular/IsRecommended**: Indică dacă cartea este recomandată/populară.
- **CoverImage**: Calea către imaginea cărții.

---

## Tabelul Borrowings

Stochează informații despre cărțile împrumutate:

- **BorrowId** (cheie primară): Identificatorul unic al împrumutului.
- **BorrowDate**: Data la care s-a făcut împrumutul.
- **ReturnDate**: Data la care s-a făcut returul.
- **Status**: Statusul împrumutului (activ, împrumutat/returnat).
- **UserId** (cheie străină): Identificatorul utilizatorului care a împrumutat cartea.
- **BookId** (cheie străină): Identificatorul cărții împrumutate.

---

## Tabelul UserProfile

Tabelul în care regăsim datele despre un utilizator:

- **ProfileId** (cheie primară): Identificator unic pentru profil.
- **Address**: Adresa utilizatorului.
- **PhoneNumber**: Numărul de telefon al utilizatorului.
- **DateOfBirth**: Data nașterii a utilizatorului.
- **IdUser** (cheie străină): Referință către utilizator.

---

## Tabelul Authors

Acest tabel păstrează informațiile esențiale despre autori:

- **AuthorId** (cheie primară): Identificator unic pentru autor.
- **FirstName**: Numele autorului.
- **LastName**: Prenumele autorului.

---

## Tabelul Categories

Acest tabel păstrează informațiile esențiale despre categorii:

- **CategoryId** (cheie primară): Identificator unic pentru categorie.
- **CategoryName**: Denumirea categoriei.

---

### Tipuri de Utilizatori

#### Utilizator

Un utilizator poate:

- Să se înregistreze și să se autentifice în aplicație.
- Să vizualizeze și să împrumute cărți.
- Să scrie recenzii și să își creeze un profil.

#### Guest (Vizitator)

Un vizitator poate:

- Să vizualizeze cărți.

#### Admin

Un administrator poate:

- Să vizualizeze cărți.
- Să adauge o nouă carte.
- Să adauge un nou autor.
- Să adauge o nouă categorie.
- Să steargă o carte.
