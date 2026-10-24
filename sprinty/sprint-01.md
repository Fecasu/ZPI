# Sprint 01 — fundament aplikacji WebStore

## Stan

| Pole | Wartość |
| --- | --- |
| Data rozpoczęcia i aktualizacji | 2026-10-24, Europe/Warsaw |
| Etap | `CZEKA_NA_AKCEPTACJE_KODU` |
| Aktywna rola | Lider: ocena rewizji kodu K1 |
| Rewizja stories | `US3` — zaakceptowana w całości |
| Rewizja kodu | `K1` — commit `8108edc7299bc424649a72110e9a5d648c49efba` |
| Akceptacja stories | US3, US-01-001–010 i WU-01–07; decyzja lidera z 2026-10-24 |
| Akceptacja kodu | Brak |
| Blokada | Brak |
| Następny krok | Lider akceptuje K1 albo wskazuje poprawki; po akceptacji rozpoczyna się recenzja |

**Cel:** dostarczyć podstawę sklepu WebStore: uruchamialną solucję .NET 10 z React, model danych, trwałą bazę SQL Server, autonomiczne usługi biznesowe i testy.

**Zakres uzgodniony z liderem:** model, baza, usługi i testy oraz uruchamialny szkielet webowy. Funkcjonalne ekrany sklepu i API HTTP udostępniające usługi biznesowe nie należą do zakresu sprintu 01. Szkielet ma się uruchamiać i wyświetlać stronę React; kryteria usług będą sprawdzane bezpośrednio przez ich kontrakty i testy.

**Status analizy:** odczytano wszystkie 21 stron PDF i diagram UML na stronie 12. Lider przyjął reguły P03–P08, repozytorium, wymagania narzędziowe oraz całą rewizję US3 z gałęzią `main`. Analiza zakończona.

**Przekazanie kodu:** gotowe rozwiązanie, migracje, testy i instrukcja uruchomienia trafiają do [Fecasu/ZPI](https://github.com/Fecasu/ZPI), na zatwierdzoną gałąź `main`. Programista przygotowuje Git z zachowaniem historii i istniejących plików. Lider nie musi tworzyć plików, katalogów ani gałęzi przez interfejs GitHuba. Akceptacja kodu przed recenzją pozostaje odrębnym etapem.

## Źródła i ustalenia

### Dokument źródłowy

- Plik: [ZPI_N_Lab1.pdf](../wymagania%20funkcjonalne/ZPI_N_Lab1.pdf).
- Tytuł: „Zaawansowane Programowanie Internetowe (studia niestacjonarne), Laboratorium 1”.
- Autor wskazany w dokumencie: dr inż. Rafał Grycuk.
- Liczba stron: 21. Odwołania poniżej wskazują strony PDF liczone od 1.
- SHA-256: `7faff67c41b84a2b0e3d1b7655d57d5694c1be7a03ba7354af4934d7f49305cc`.
- Stan początkowy projektu: instrukcje ról i PDF; brak kodu aplikacji oraz lokalnego katalogu `.git` w katalogu projektu.

### Decyzje lidera z czatu — 2026-10-24

| ID | Rozstrzygnięcie | Podstawa |
| --- | --- | --- |
| D01 / P01 | Zakres: model, baza, usługi, testy i uruchamialny szkielet webowy; bez biznesowego API HTTP i ekranów React. | Odpowiedź: „Model, baza, usługi i testy oraz uruchamialny szkielet webowy”. |
| D02 / P02 | Preferowana platforma .NET 10; dopuszczony dobór innej wersji, jeśli okaże się to konieczne dla realizacji celu. Docelowo przyjmujemy `net10.0`, ponieważ SDK 10 jest dostępne lokalnie. | Lider wyjaśnił, że PDF jest stary, i wskazał .NET 10 jako preferencję. |
| D03 / P03 | Uporządkowane adresy należące do klienta albo sklepu; opcjonalne wskazania adresu dostawy i rozliczeniowego klienta. | Przyjęcie propozycji P03–P08; szczegóły w US-01-005. |
| D04 / P04 | Sklep ma nazwę, adresy i pracowników; usługa przypisuje istniejących pracowników, bez tworzenia kont. | Przyjęcie propozycji P03–P08; szczegóły w US-01-002 i US-01-006. |
| D05 / P05 | Kwota zamówienia liczona przy zapisie, bez odrębnej ceny historycznej pozycji i zmian stanów; późniejsze dane wysyłki mogą być puste. Waluta PLN, masa w kilogramach; zamówienie na fakturze nie podlega edycji. | Przyjęcie propozycji P03–P08; szczegóły w US-01-007. |
| D06 / P06 | Prosta ewidencja faktur z numerem i datą, obejmujących zamówienia jednego klienta; bez podatków, automatycznej numeracji, płatności i eksportu PDF. | Przyjęcie propozycji P03–P08; szczegóły w US-01-008. |
| D07 / P07 | Brak ID lub zero oznacza dodanie; dodatnie ID aktualizuje istniejący produkt. Ilość jest sumą stanów produktu; obowiązuje uzgodniona walidacja danych. | Przyjęcie propozycji P03–P08; szczegóły w US-01-003–004. |
| D08 / P08 | Usługi dodają/aktualizują, odczytują i filtrują; realizują uzgodnione powiązania, bez usuwania rekordów. Ujednolicone wyjątki i brak częściowego zapisu po błędzie. | Przyjęcie propozycji P03–P08; szczegóły we wspólnych kryteriach usług. |
| D09 / P09 | Repozytorium kodu: `https://github.com/Fecasu/ZPI`. Wcześniejsza tablica Projects nie zastępuje tego adresu. | Lider podał adres repozytorium w odpowiedzi na pytanie o miejsce przechowywania kodu. |
| D10 / P09 | Rozszerzenia VS Code opisane w PDF nie są warunkiem odbioru; wymagane są działające rozwiązanie, migracje i testy. | Odpowiedź: „Wystarczy działające rozwiązanie, migracje i testy”. |
| D11 / P09 | Gałąź docelowa: `main` w `Fecasu/ZPI`. | Lider zaakceptował US3 oraz main; pełny zapis poniżej. |

Podstawa decyzji D03–D08: odpowiedź lidera projektu w czacie z 2026-10-24: „Przyjmuję propozycje P03–P08 opisane w karcie”. Dotyczy propozycji z rewizji US1. Szczegółową treść przeniesiono do aktualnych kryteriów, zachowując powiązanie z tą decyzją. Te odpowiedzi rozstrzygają pytania; nie są akceptacją rewizji stories ani kodu.

### Rozpoznanie techniczne

Na etapie analizy kontrola tylko do odczytu wykazała: `dotnet --list-sdks` zwraca `10.0.401`, a `SqlLocalDB versions` — `Microsoft SQL Server 2025 (17.0.4025.3)`. Polecenia Git, Node.js i VS Code są dostępne. Wyniki rzeczywistego budowania, migracji i uruchomienia wykonanych przez programistę zapisano w sekcji implementacji.

Wersje `.NET 5`, `net6.0` i bibliotek w PDF stanowią materiał historyczny; spójny zestaw zależności dla .NET 10 dobiera programista w ramach D02. EF Core 10 wymaga .NET 10 — [dokumentacja Microsoft](https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-10.0/whatsnew). Dawne szablony SPA są oznaczone jako historyczne; aktualne rozwiązania ASP.NET Core wspierają React i współczesne narzędzia frontendowe — [dokumentacja Microsoft](https://learn.microsoft.com/en-us/aspnet/core/client-side/spa/intro?view=aspnetcore-10.0). Nie ma podstaw do traktowania starego polecenia `dotnet new react -f net6.0` jako niezmiennego warunku odbioru; istotne są uzgodniona platforma, podział warstw i uruchamialny szkielet.

### Model ze źródła i przyjętych ustaleń

Diagram ze strony 12 należy czytać wraz z poleceniami rozszerzającymi go na stronach 12–13. Pola adresowe i kategoria będące na rysunku napisami mają zostać zastąpione relacjami do encji.

| Encja | Dane i powiązania wskazane w PDF |
| --- | --- |
| `User` | Dziedziczy po `IdentityUser<int>`; `FirstName: string`, `LastName: string`, `RegistrationDate: DateTime`. |
| `Customer` | Dziedziczy po `User`; kolekcja zamówień; adres rozliczeniowy i dostawy mają korzystać z obiektów `Address` zamiast `string`. |
| `Supplier` | Dziedziczy po `User`; kolekcja produktów. |
| `Product` | `Id: int`, `Name: string`, `Description: string`, `ImageBytes: byte[]`, `Price: decimal`, `Weight: float`; dostawca i kategoria jako encje. Przykłady zawierają `SupplierId` i `CategoryId`. |
| `ProductStock` | Produkt i nieujemne `Quantity: int`; ilość w `ProductVm` jest sumą stanów produktu. Bez dodatkowej relacji stan–sklep i osobnej usługi stanów (D07). |
| `Order` | `Id: int`, klient, `OrderDate: DateTime`, `DeliveryDate: DateTime?`, `TotalAmount: decimal`, `TrackingNumber: long?`; produkty i opcjonalna faktura. Opcjonalność danych wysyłki wynika z D05. |
| `OrderProduct` | Encja asocjacyjna: zamówienie, produkt i `Quantity: int`. |
| `Category` | `Id`, wymagane `Name` i `Tag` (przykład na s. 18 i D07). |
| `Address` | Kraj, kod pocztowy, miejscowość i numer budynku wymagane; ulica i numer lokalu opcjonalne. Właścicielem jest dokładnie jeden klient albo jeden sklep (D03). |
| `StationaryStore` | ID, wymagana nazwa, kolekcje adresów i pracowników (D04). |
| `StationaryStoreEmployee` | Dziedziczy po `User`; należy do jednego sklepu. Bez funkcji kadrowych i zarządzania kontami w usłudze sklepu (D04). |
| `Invoice` | ID, wymagany unikalny numer podany przez wywołującego, data wystawienia, co najmniej jedno zamówienie tego samego klienta; kwota jest sumą kwot zamówień (D06). |

### Wymagane relacje danych

Źródło: strona 12, rozdział 4 pkt 1, i strona 13, ciąg dalszy rozdziału 4.

| Strona „jeden” | Strona „wiele” |
| --- | --- |
| `Customer` | `Order` |
| `Supplier` | `Product` |
| `Product` | `ProductStock` |
| `Customer` | `Address` |
| `Category` | `Product` |
| `Invoice` | `Order` |
| `StationaryStore` | `Address` |
| `StationaryStore` | `StationaryStoreEmployee` |

Ponadto: `Order` ↔ `Product` to N:M poprzez `OrderProduct`; `User`, `Customer`, `Supplier`, `StationaryStoreEmployee` mają być mapowane metodą TPH. Relacja `Invoice` → `Order` pozostaje 1:M. Zamówienie może nie mieć faktury; faktura wymaga co najmniej jednego zamówienia. Powiązania i ich zmiany podlegają D03–D08 oraz kryteriom właściwych stories.

## User stories — US3

Role biznesowe w opisach wskazują perspektywę i korzyść. Nie ustanawiają nowych uprawnień, systemu logowania ani ekranów. Stories US-01-001, US-01-009 i US-01-010 opisują wymagane rezultaty techniczne wspierające funkcje sklepu. Kryteria usługi obejmują również wszystkie mające do niej zastosowanie wspólne kryteria WU-01–WU-07 opisane dalej. Szczegółowe rozstrzygnięcia lidera mają pierwszeństwo przed sprzecznymi przykładami w PDF.

### US-01-001 — Uruchomienie podstawy aplikacji WebStore

**Opis:** Jako członek zespołu rozwijającego WebStore chcę zbudować i uruchomić aplikację z rozdzielonymi warstwami, aby rozwijać sklep na działającej podstawie klient–serwer.

**Źródła:** s. 2–12, rozdziały 1–2 i rozdział 3 pkt 1; s. 21, przekazanie kodu; D01, D02, D09–D11.

**Zakres:** solucja `WebStore`, cztery biblioteki `WebStore.Model`, `WebStore.DAL`, `WebStore.Services`, `WebStore.ViewModels`, warstwa ASP.NET Core z React w `WebStore.Web`; osobny projekt testów opisany w US-01-010. Bez biznesowych ekranów i endpointów w tym sprincie.

**Kryteria akceptacji:**

- **US-01-001-KA-01:** Mając źródła i wymagane zależności, po zbudowaniu solucji poleceniem `dotnet build` wszystkie projekty .NET kompilują się dla `net10.0` bez błędów.
- **US-01-001-KA-02:** Przy inspekcji referencji `DAL` odwołuje się do `Model`, `ViewModels` do `Model`, `Services` do `Model`, `DAL`, `ViewModels`, a `Web` do tych czterech warstw.
- **US-01-001-KA-03:** Po wykonaniu udokumentowanej procedury uruchomienia serwer ASP.NET Core i klient React startują, a strona szkieletu jest dostępna w przeglądarce.
- **US-01-001-KA-04:** Przekazany kod i dokumentacja pozwalają odtworzyć budowanie, migrację, uruchomienie i testy bez obowiązku instalowania rozszerzeń VS Code opisanych w PDF. Potrzebne SDK, silnik bazy i zależności projektu pozostają wymaganiami uruchomienia.
- **US-01-001-KA-05:** Po przyjęciu wariantu US3 rezultat sprintu jest zapisany w repozytorium `Fecasu/ZPI` na gałęzi `main`, a karta sprintu identyfikuje przekazany commit. Repozytorium zawiera źródła, migracje, testy i instrukcję uruchomienia. Zdalna zawartość jest sprawdzana przed połączeniem i publikacją; istniejące pliki nie są nadpisywane bez rozpoznania ich znaczenia.

**Zależności:** Brak dla utworzenia szkieletu; pełne zbudowanie końcowego sprintu obejmuje pozostałe stories.

**Sposób dostarczenia:** przyjęty wraz z US3; konfigurację wykonuje programista.

### US-01-002 — Rozróżnianie klientów, dostawców i pracowników sklepu

**Opis:** Jako zespół rozwijający sklep chcę przechowywać wspólne dane użytkowników wraz z ich typem, aby wiązać zamówienia, produkty i pracowników z właściwymi osobami.

**Źródła:** s. 12, diagram i rozdział 3 pkt 4; s. 13, wymaganie TPH; s. 18–19, przykład integracji Identity; D04, D08.

**Zakres:** model tożsamości i powiązania encji; bez procesów logowania, rejestracji przez stronę i zarządzania uprawnieniami.

**Kryteria akceptacji:**

- **US-01-002-KA-01:** Przy inspekcji modelu `User` dziedziczy po `IdentityUser<int>` i zawiera imię, nazwisko oraz datę rejestracji; `Customer`, `Supplier` i `StationaryStoreEmployee` dziedziczą po `User`.
- **US-01-002-KA-02:** Po zapisaniu i ponownym odczycie klienta, dostawcy oraz pracownika zachowane są ich typy i dane wspólne.
- **US-01-002-KA-03:** Klient może być powiązany z wieloma zamówieniami, a dostawca z wieloma produktami; odczyt wskazuje właściwego właściciela każdego powiązania.
- **US-01-002-KA-04:** Po zapisaniu pracownika jest on powiązany z dokładnie jednym istniejącym sklepem i zachowuje dane odziedziczone po `User`. Konta potrzebne do testów i demonstracji mogą być przygotowane w danych testowych; ich tworzenie nie jest operacją `StoreService`.

**Zależności:** US-01-001. Model użytkowników powstaje przed migracją; trwały odczyt i TPH są odbierane wspólnie z US-01-009.

### US-01-003 — Dodawanie i aktualizacja produktu

**Opis:** Jako dostawca chcę zapisać nowy produkt i zmienić dane istniejącego, aby oferta sklepu zawierała aktualne informacje.

**Źródła:** s. 12, model i rozdział 3 pkt 5; s. 14–17, `ProductVm`, `AddOrUpdateProductVm`, `IProductService`, `ProductService`; s. 20–21, testy dodania i aktualizacji; D05, D07, D08.

**Zakres:** metoda `AddOrUpdateProduct`, nazwa, opis, obraz, cena, masa, kategoria i dostawca; mapowanie VM ↔ encja, zapis do bazy i zwrócenie `ProductVm`.

**Kryteria akceptacji:**

- **US-01-003-KA-01:** Mając istniejącą kategorię i dostawcę, po dodaniu produktu z `Id = null` albo `Id = 0` zapisany produkt zawiera przekazane dane i oba powiązania; zwrócony VM odzwierciedla dane produktu.
- **US-01-003-KA-02:** Mając istniejący produkt, po aktualizacji przez jego identyfikator i zmianie ceny ponowny odczyt pokazuje nową cenę tego produktu bez utworzenia drugiego produktu.
- **US-01-003-KA-03:** Po wywołaniu `AddOrUpdateProduct(null)` usługa zgłasza `ArgumentNullException` i nie zapisuje produktu.
- **US-01-003-KA-04:** `AddOrUpdateProductVm` zawiera opcjonalne `Id` oraz wskazane w PDF wymagane pola; błędy operacji są logowane i przekazywane wywołującemu zgodnie z kontraktem usługi.
- **US-01-003-KA-05:** Brak wymaganej nazwy, opisu lub obrazu albo cena lub masa niedodatnia powodują `ArgumentException` bez zapisu. Kategoria i dostawca muszą istnieć; ich nieistniejące identyfikatory powodują `KeyNotFoundException`. Cena jest w PLN, masa w kilogramach.
- **US-01-003-KA-06:** Dodatnie ID nieistniejącego produktu powoduje `KeyNotFoundException`, a ujemne ID — `ArgumentException`; żaden z tych przypadków nie tworzy nowego produktu.
- **US-01-003-KA-07:** Dane kategorii zawierają ID i niepuste `Name` oraz `Tag`. Powiązanie produktu wskazuje kategorię jako encję, a nie nazwę kategorii zapisaną w produkcie.

**Zależności:** US-01-001, US-01-002, US-01-009; kategoria jest częścią modelu tej story.

### US-01-004 — Pobieranie i filtrowanie produktów

**Opis:** Jako klient sklepu chcę pobierać informacje o produktach i wybierać produkty spełniające warunek, aby znaleźć interesującą mnie ofertę.

**Źródła:** s. 14–17, `ProductVm`, `GetProduct`, `GetProducts`; s. 20, testy odczytu; D07, D08.

**Zakres:** pobieranie pojedynczego produktu przez predykat oraz kolekcji z opcjonalnym filtrem na poziomie usługi; bez wyszukiwarki ekranowej, paginacji i ustalania nowego sortowania.

**Kryteria akceptacji:**

- **US-01-004-KA-01:** Mając produkt pasujący do przekazanego filtra, `GetProduct` zwraca jego dane w `ProductVm`.
- **US-01-004-KA-02:** Mając produkty spełniające i niespełniające filtra, `GetProducts` z filtrem zwraca wszystkie i wyłącznie produkty spełniające ten filtr.
- **US-01-004-KA-03:** Przy braku filtra `GetProducts` zwraca wszystkie zapisane produkty, w tym dokładnie dwa produkty w izolowanym zestawie danych zawierającym dwa produkty.
- **US-01-004-KA-04:** `GetProduct(null)` zgłasza `ArgumentNullException`; `ProductVm` udostępnia nazwę, opis, obraz, cenę, masę i pole `Quantity` wskazane w PDF.
- **US-01-004-KA-05:** Gdy żaden produkt nie pasuje do filtra, `GetProduct` zwraca `null`, a `GetProducts` pustą kolekcję. Odczyt wszystkich produktów z pustej bazy również zwraca pustą kolekcję.
- **US-01-004-KA-06:** `ProductVm.Quantity` jest sumą ilości ze wszystkich rekordów `ProductStock` danego produktu; przykładowo stany 2 i 3 dają wynik 5, a brak stanów daje 0. Zarówno pojedynczy odczyt, jak i lista stosują tę regułę.

**Zależności:** US-01-003, US-01-009.

### US-01-005 — Przechowywanie i obsługa adresów

**Opis:** Jako klient sklepu chcę, aby mój adres rozliczeniowy i dostawy był przechowywany jako uporządkowany obiekt, aby aplikacja mogła jednoznacznie korzystać z danych adresowych.

**Źródła:** s. 12, rozdział 3 pkt 3; s. 13, relacje `Customer–Address`, `StationaryStore–Address`; s. 21, `AddressService`; D03, D08.

**Zakres:** encja `Address`, powiązania adresów klienta i sklepu, autonomiczny `AddressService` i jego VM.

**Kryteria akceptacji:**

- **US-01-005-KA-01:** Przy inspekcji modelu adresy rozliczeniowy i dostawy klienta odwołują się do obiektów `Address`, zamiast przechowywać pełny adres jako pojedynczy napis.
- **US-01-005-KA-02:** Mając klienta z wieloma adresami, po zapisie i odczycie możliwe jest odtworzenie przypisania tych adresów do tego klienta zgodnie z relacją 1:M.
- **US-01-005-KA-03:** `AddressService` ma publiczny interfejs, implementację dziedziczącą po `BaseService` oraz wejściowe i wynikowe VM w warstwie `ViewModels`.
- **US-01-005-KA-04:** Dodanie i aktualizacja zapisują kraj, kod pocztowy, miejscowość, ulicę, numer budynku i numer lokalu. Ulica i numer lokalu mogą być puste; brak pozostałych wymaganych danych powoduje `ArgumentException` bez zapisu.
- **US-01-005-KA-05:** Adres ma dokładnie jednego właściciela: istniejącego klienta albo istniejący sklep. Brak właściciela lub wskazanie obu typów naraz powoduje `ArgumentException`; nieistniejący ID właściciela powoduje `KeyNotFoundException`.
- **US-01-005-KA-06:** Usługa pozwala wskazać i odczytać adres rozliczeniowy i dostawy klienta spośród jego adresów. Wskazania mogą być puste i mogą prowadzić do tego samego adresu. Próba wskazania adresu innego klienta lub sklepu powoduje `ArgumentException` bez zmiany wcześniejszych wskazań.
- **US-01-005-KA-07:** Zmiana właściciela adresu używanego jako rozliczeniowy lub dostawy powoduje `ArgumentException` i pozostawia wcześniejsze powiązania. Gdy adres nie jest tak używany, jego właściciel może zostać zmieniony z zachowaniem zasady jednego istniejącego właściciela.

**Zależności:** US-01-002, US-01-009; powiązanie ze sklepem w US-01-006.

### US-01-006 — Ewidencja sklepów stacjonarnych i pracowników

**Opis:** Jako pracownik sklepu chcę, aby aplikacja przechowywała sklep, jego adresy i przypisanych pracowników, aby odróżniać placówki i powiązane z nimi osoby.

**Źródła:** s. 12, rozdział 3 pkt 4; s. 13, relacje sklep–adres i sklep–pracownik; s. 21, `StoreService`; D03, D04, D08.

**Zakres:** encje `StationaryStore`, `StationaryStoreEmployee`, ich powiązania i autonomiczna usługa sklepu.

**Kryteria akceptacji:**

- **US-01-006-KA-01:** Model zawiera `StationaryStore` i `StationaryStoreEmployee`, a pracownik dziedziczy po `User`.
- **US-01-006-KA-02:** Mając sklep z wieloma adresami i pracownikami, zapis i ponowny odczyt zachowują wszystkie te przypisania zgodnie z relacjami 1:M.
- **US-01-006-KA-03:** `StoreService` ma publiczny interfejs, implementację dziedziczącą po `BaseService` i odpowiednie VM; możliwe jest utworzenie usługi z zależnościami przez mechanizm DI.
- **US-01-006-KA-04:** Dodanie i aktualizacja sklepu zapisują jego wymaganą niepustą nazwę. Odczyt sklepu udostępnia jego ID, nazwę i powiązania z adresami oraz pracownikami.
- **US-01-006-KA-05:** Przypisanie istniejącego pracownika do istniejącego sklepu i zmiana tego przypisania zachowują dokładnie jeden sklep pracownika. Po przeniesieniu pracownik należy do nowego sklepu i nie należy do poprzedniego. Nieistniejące ID powoduje `KeyNotFoundException`, a wskazanie użytkownika innego typu zamiast pracownika — `ArgumentException`.

**Zależności:** US-01-002, US-01-005, US-01-009.

### US-01-007 — Obsługa zamówienia klienta z produktami

**Opis:** Jako klient sklepu chcę mieć zapisane zamówienie z produktami i ich ilościami, aby aplikacja mogła odtworzyć zawartość mojego zamówienia.

**Źródła:** s. 12, diagram `Order`, `OrderProduct`; s. 12–13, relacje klient–zamówienie i zamówienie–produkt; s. 21, `OrderService`; D05, D06, D08.

**Zakres:** zamówienie, klient, produkty, ilości, dane terminów, kwoty i numeru przesyłki wskazane w modelu oraz autonomiczna usługa zamówień.

**Kryteria akceptacji:**

- **US-01-007-KA-01:** Model zamówienia zawiera identyfikator, istniejącego klienta, wymaganą datę zamówienia, opcjonalną datę dostawy, kwotę całkowitą i opcjonalny numer śledzenia. Dane wysyłki mogą zostać uzupełnione później; data dostawy wcześniejsza niż data zamówienia powoduje `ArgumentException`.
- **US-01-007-KA-02:** Po zapisaniu zamówienia zawierającego różne produkty ponowny odczyt odtwarza produkty i ilości z encji `OrderProduct`.
- **US-01-007-KA-03:** Ten sam produkt może należeć do wielu zamówień, a jedno zamówienie może zawierać wiele produktów; powiązanie jest realizowane przez `OrderProduct`, nie przez pojedyncze ID produktu w zamówieniu.
- **US-01-007-KA-04:** `OrderService` ma publiczny interfejs, implementację dziedziczącą po `BaseService` i odpowiednie VM.
- **US-01-007-KA-05:** Zamówienie zawiera co najmniej jedną pozycję z istniejącym produktem i dodatnią ilością całkowitą. Pusta lista, niedodatnia ilość lub powtórzony produkt powodują `ArgumentException`; nieistniejący klient albo produkt — `KeyNotFoundException`. Błąd nie zapisuje zamówienia ani części pozycji.
- **US-01-007-KA-06:** Usługa liczy i zapisuje `TotalAmount` w PLN jako sumę aktualnych cen produktów pomnożonych przez ilości. Dwie sztuki po 10 PLN i jedna po 15 PLN dają 35 PLN. Późniejsza zmiana ceny katalogowej nie zmienia zapisanej kwoty; edycja zamówienia przelicza ją według cen obowiązujących przy tej edycji. Pozycje nie mają odrębnej ceny historycznej w tym sprincie.
- **US-01-007-KA-07:** Zapis i edycja zamówienia pozostawiają ilości `ProductStock` bez zmian.
- **US-01-007-KA-08:** Próba edycji zamówienia przypisanego do faktury powoduje `ArgumentException` bez zapisu. Edycja jest możliwa po usunięciu powiązania przez usługę faktur, przy zachowaniu jej reguł.

**Zależności:** US-01-002, US-01-003, US-01-009.

### US-01-008 — Powiązanie faktury z zamówieniami

**Opis:** Jako klient sklepu chcę, aby aplikacja wiązała fakturę z moimi zamówieniami, aby można było ustalić, których zamówień dotyczy dokument.

**Źródła:** s. 12, rozdział 3 pkt 6; s. 13, `Invoice–Order` 1:M; s. 21, `InvoiceService`; D05, D06, D08.

**Zakres:** encja `Invoice`, zmiana modelu zamówienia, relacja 1:M i autonomiczna usługa faktur. Zgodnie z D06 jest to prosta ewidencja na potrzeby ćwiczenia, bez podatków, automatycznej numeracji, płatności i eksportu PDF.

**Kryteria akceptacji:**

- **US-01-008-KA-01:** Model zawiera `Invoice`, a `Order` ma powiązanie z fakturą.
- **US-01-008-KA-02:** Mając jedną fakturę przypisaną do kilku zamówień, po zapisie i ponownym odczycie wszystkie wskazane zamówienia są powiązane z tą fakturą.
- **US-01-008-KA-03:** `InvoiceService` ma publiczny interfejs, implementację dziedziczącą po `BaseService` i odpowiednie VM.
- **US-01-008-KA-04:** Faktura zapisuje ID, wymagany niepusty numer podany przez wywołującego i datę wystawienia. Numer musi być unikalny wśród innych faktur; powtórzenie numeru powoduje `ArgumentException` bez zapisu.
- **US-01-008-KA-05:** Faktura wymaga co najmniej jednego istniejącego zamówienia, a wszystkie jej zamówienia należą do jednego klienta. Pusty zestaw lub różni klienci powodują `ArgumentException`; nieistniejące zamówienie — `KeyNotFoundException`. Zamówienie może nie mieć faktury przed przypisaniem, lecz nie może należeć do dwóch faktur.
- **US-01-008-KA-06:** Kwota faktury jest sumą zapisanych `TotalAmount` jej zamówień. Dla zamówień na 35 i 20 PLN odczyt faktury pokazuje 55 PLN.
- **US-01-008-KA-07:** Usługa pozwala aktualizować dane faktury i jej zestaw zamówień. Usunięcie powiązania zwalnia zamówienie z tej faktury; zestaw końcowy nadal musi zawierać co najmniej jedno zamówienie jednego klienta. Próba przypisania zamówienia należącego do innej faktury powoduje `ArgumentException`; błąd pozostawia poprzedni zestaw powiązań i dane bez zmian.

**Zależności:** US-01-007, US-01-009.

### US-01-009 — Trwały zapis modelu i relacji w bazie

**Opis:** Jako zespół rozwijający sklep chcę utworzyć bazę z modelu i zachować dane wraz z relacjami, aby kolejne etapy aplikacji korzystały ze spójnej, trwałej podstawy.

**Źródła:** s. 2, SQL Server LocalDB dla Windows; s. 10, EF Core i dostawca SQL Server; s. 12–13, rozdział 4; s. 15–16, `ApplicationDbContext`; D03–D08.

**Zakres:** `ApplicationDbContext` w `WebStore.DAL`, konfiguracja EF Core, migracja i jej zastosowanie do SQL Server LocalDB, wszystkie relacje wymienione w źródle.

**Kryteria akceptacji:**

- **US-01-009-KA-01:** Po zastosowaniu dostarczonej migracji do pustej bazy SQL Server powstaje schemat modelu, a operacja kończy się powodzeniem.
- **US-01-009-KA-02:** Inspekcja modelu EF i utworzonego schematu potwierdza osiem relacji 1:M wskazanych w tabeli relacji oraz N:M przez `OrderProduct`.
- **US-01-009-KA-03:** Schemat i odczyt danych potwierdzają dziedziczenie TPH dla `User`, `Customer`, `Supplier` i `StationaryStoreEmployee`.
- **US-01-009-KA-04:** Po zapisie danych i ponownym utworzeniu kontekstu połączenie z tą samą bazą zwraca zachowane dane i ich powiązania. Baza InMemory użyta w testach nie stanowi dowodu zastosowania migracji do SQL Server.
- **US-01-009-KA-05:** Model i zapis danych obsługują opcjonalną fakturę zamówienia, opcjonalne dane wysyłki oraz wskazania adresów klienta. Adres zachowuje dokładnie jednego właściciela, a pracownik dokładnie jeden sklep, zgodnie z właściwymi kryteriami usług.
- **US-01-009-KA-06:** Zapis stanu produktu nie dopuszcza ujemnego `ProductStock.Quantity`. Dla stanu równego zero zapis jest poprawny. Kolejne odczyty produktu zachowują uzgodnioną sumę stanów.

**Zależności:** US-01-001, model użytkowników z US-01-002 oraz decyzje D03–D08. Schemat obejmuje również encje opisane w US-01-003–008; nie wymaga ukończonych usług z tych stories. Kolejność techniczna to modele → DAL i migracja → usługi wraz z testami.

### US-01-010 — Automatyczne sprawdzanie usług

**Opis:** Jako osoba weryfikująca WebStore chcę uruchomić testy usług biznesowych, aby potwierdzić działanie uzgodnionych operacji i wykrywać regresje.

**Źródła:** s. 17–21, rozdziały 5.2–5.4; D02–D08.

**Zakres:** `WebStore.Tests` w solucji, testy produktu oraz `OrderService`, `InvoiceService`, `StoreService`, `AddressService`. xUnit jest wskazanym w PDF wariantem; testy mają działać na docelowej platformie .NET 10.

**Kryteria akceptacji:**

- **US-01-010-KA-01:** `dotnet test` wykrywa i wykonuje testy wszystkich pięciu usług, a wyniki wskazują rzeczywistą liczbę testów zakończonych sukcesem i niepowodzeniem.
- **US-01-010-KA-02:** Dla produktu istnieją testy odczytu jednego produktu, filtrowania kolekcji, pobrania wszystkich, dodania i aktualizacji zgodne ze scenariuszami na s. 20–21 oraz testy uzgodnionych błędów i przypadków brzegowych.
- **US-01-010-KA-03:** Każda uzgodniona operacja czterech pozostałych usług ma test scenariusza poprawnego oraz uzgodnionych błędów; przypadki są powiązane z kryteriami stories.
- **US-01-010-KA-04:** Wynik testu nie zależy od uruchomienia wcześniejszego testu ani od danych pozostawionych przez inne testy; test odczytu wszystkich produktów porównuje rezultat z danymi wejściowymi zestawu testowego, a nie z tym samym wynikiem po obu stronach asercji.
- **US-01-010-KA-05:** Usługi są tworzone wraz z kontekstem, mapowaniem i logowaniem; testy potwierdzają poprawne przejście danych przez VM i encje. Końcowy wynik zaakceptowanego zakresu nie zawiera pominiętych kontroli wymaganych kryteriami.
- **US-01-010-KA-06:** Zestaw testów sprawdza wspólne kryteria WU-01–WU-07 dla każdej z pięciu usług oraz operacje powiązań: adresy klienta, przypisanie pracownika i zestaw zamówień faktury. Przy błędzie operacji test porównuje stan przed i po wywołaniu, potwierdzając brak częściowego zapisu.

**Zależności:** US-01-001–009; testy powinny powstawać razem z realizowanymi funkcjonalnościami, a pełny zestaw jest wynikiem końcowym.

## Wspólne kryteria usług

Źródło: D07–D08 oraz przykład `ProductService` z PDF. Każde kryterium sprawdza się dla wszystkich usług, których dotyczy; spełnienie go przez samą usługę produktów nie wystarcza. Nazwy metod czterech nowych usług i szczegóły techniczne VM dobiera programista zgodnie z konwencjami projektu i poniższym zachowaniem.

| ID | Mierzalny warunek |
| --- | --- |
| WU-01 | Każda z pięciu usług udostępnia dodanie/aktualizację, pobranie pojedynczego obiektu przez filtr oraz listy z opcjonalnym filtrem. Operacje zapisują i zwracają dane zgodne z właściwą story. Usuwanie rekordów nie należy do zakresu. |
| WU-02 | W dodaniu/aktualizacji brak ID albo zero tworzy rekord, dodatnie ID aktualizuje istniejący, nieistniejące dodatnie ID powoduje `KeyNotFoundException`, a ujemne — `ArgumentException`. |
| WU-03 | Odczyt pojedynczy zwraca pierwszy pasujący obiekt albo `null`. Lista z filtrem zawiera wszystkie i wyłącznie pasujące obiekty; lista bez filtra — wszystkie. Brak wyników daje pustą kolekcję. Nie ustala się dodatkowego sortowania ani paginacji. |
| WU-04 | `null` wejściowego VM lub wymaganego predykatu powoduje `ArgumentNullException`. Opcjonalny filtr kolekcji może być `null`. |
| WU-05 | Niepoprawne dane i naruszenie reguł biznesowych powodują `ArgumentException`. Wskazanie nieistniejącego ID powoduje `KeyNotFoundException`; istniejący rekord niewłaściwego typu nie tworzy powiązania. Wymagane napisy nie mogą być puste. Nie dodaje się arbitralnych limitów biznesowych długości ani wartości. |
| WU-06 | Niepoprawna operacja nie pozostawia częściowego zapisu danych ani powiązań. Operacje na adresach, pracownikach i fakturach zachowują reguły US-01-005–008 również przy zmianie istniejących powiązań. |
| WU-07 | Usługi korzystają z wejściowych i wynikowych VM, mapowania, kontekstu i loggera zgodnie z `BaseService`; wyniki oraz błędy są dostępne wywołującemu. Weryfikacja logowania błędów nie zastępuje sprawdzenia zwracanego wyniku lub wyjątku. |

## Wspólne warunki techniczne i pokrycie PDF

Wszystkie pięć usług ma interfejs `I…Service` w `WebStore.Services/Interfaces`, implementację w `ConcreteServices` i bazę `BaseService` z kontekstem, mapperem i loggerem (s. 13–16, 21). VM znajdują się w `WebStore.ViewModels/VM`, z sufiksem `Vm`. Mapowanie i rejestracja DI muszą wystarczać do uruchomienia usług i testów. To warunki techniczne istniejących stories, nie odrębne funkcjonalności użytkownika.

| Fragment PDF | Pokrycie / sposób uwzględnienia |
| --- | --- |
| s. 2–4: SDK, SQL Server, VS Code i rozszerzenia | US-01-001, US-01-009; D02 rozstrzyga platformę, D10 wyłącza obowiązek rozszerzeń edytora z odbioru. |
| s. 4–7 i s. 21: repozytorium, gałąź studenta, Commit & Push | D09 i D11: `Fecasu/ZPI`, przekazanie na zaakceptowaną gałąź `main`. |
| s. 7–12: solucja, warstwy, referencje, build i uruchomienie | US-01-001; osobny projekt testów w US-01-010. |
| s. 12: model bazowy i jego rozszerzenia | US-01-002–008; diagram odczytany wizualnie, nie tylko przez ekstrakcję tekstu. |
| s. 12–13: relacje, TPH, migracje | US-01-009 wraz z kryteriami powiązań w stories domenowych. |
| s. 13–17: VM, BaseService, ProductService, logowanie i mapowanie | US-01-003–004 oraz wspólne warunki techniczne usług. |
| s. 17–21: infrastruktura i scenariusze testów | US-01-010. |
| s. 21: OrderService, InvoiceService, StoreService, AddressService | US-01-005–008 oraz WU-01–WU-07; reguły przyjęte w D03–D08. |

### Niespójności przykładów wymagające uwagi

- Wersje .NET 5/6 rozstrzygnięto decyzją D02 na korzyść .NET 10.
- Kod na s. 16 kieruje `Id = 0` do `Update`, podczas gdy opis na s. 17 mówi o dodaniu. D07 rozstrzyga: zero oznacza dodanie.
- `ProductVm.Quantity` występuje na s. 14, ale `Product` nie ma takiego pola. D07 rozstrzyga: sumowanie `ProductStock.Quantity`, a przy braku stanów zero.
- Przykład `GetAllProductsTest` porównuje `products.Count()` z nim samym. Kryterium testu musi porównać odczyt z przygotowanymi danymi; tautologia nie dowodzi poprawności.
- Przykład inicjalizacji danych zawiera `async void`, a testy korzystają ze wspólnej bazy InMemory. Programista ma zapewnić zakończenie przygotowania danych przed testem i izolację danych; kopiowanie tego fragmentu nie zastępuje wymagania powtarzalnych testów.

## Pytania otwarte do lidera

Brak.

## Akceptacje

- Rodzaj: **STORIES**, aktualna.
- Lider: lider projektu — użytkownik czatu.
- Data decyzji: 2026-10-24, Europe/Warsaw. Zapisano o 11:19 (czas odczytu decyzji; dokładny czas wysłania wiadomości nie jest dostępny).
- Cytat: „Akceptuję US3 dla sprintu 01 oraz gałąź main w repozytorium Fecasu/ZPI”.
- Zakres: kompletna **US3**, **US-01-001–US-01-010**, **WU-01–WU-07**, gałąź **main** w **Fecasu/ZPI**.
- Akceptacja kodu: jeszcze nieudzielona.

## Implementacja

### Rewizja do akceptacji

**K1:** [commit 8108edc7299bc424649a72110e9a5d648c49efba](https://github.com/Fecasu/ZPI/commit/8108edc7299bc424649a72110e9a5d648c49efba). Zakres: wszystkie stories US3, czyli US-01-001–US-01-010 oraz WU-01–WU-07. Commit zawiera kod, migrację, testy, instrukcję uruchomienia, dokumenty ról i dowody testów. Późniejszy commit aktualizujący tę kartę opisuje tę samą K1 i nie zmienia kodu ani testów.

Repozytorium: [Fecasu/ZPI, main](https://github.com/Fecasu/ZPI/tree/main). Zdalny commit początkowy `182b88d999c5af4548e5e57c2193c7d6ea8c148c` nie zawierał plików; jego historia została zachowana. Przed przygotowaniem K1 ponownie pobrano `origin/main` i potwierdzono brak nowych zmian. Autor Git został skonfigurowany lokalnie według danych podanych przez lidera w czacie.

### Dostarczone rozwiązanie

- .NET 10, EF Core i Identity 10.0.13; SDK użyte do weryfikacji: 10.0.401.
- Sześć projektów w `WebStore.sln`: pięć warstw oraz testy. Referencje odpowiadają US-01-001.
- SQL Server LocalDB 2025, baza aplikacji `ZpiWebStore`, migracja `20261024122406_InitialCreate`, osiem relacji 1:M, N:M przez `OrderProduct` i TPH użytkowników.
- Pięć usług z publicznymi interfejsami, VM, mapowaniem i loggerem. Zapisy obejmują walidację oraz transakcję `Serializable`; błędy wycofują zapis i czyszczą śledzenie zmian.
- Jawny `StoreMapper` realizuje mapowanie VM ↔ encja; jest decyzją implementacyjną w zatwierdzonym zakresie, bez uzależniania projektu od historycznej wersji biblioteki z PDF.
- React 19.3.1 i Vite 7.3.7, uruchomienie przez ASP.NET Core pod `http://localhost:5100`. Klient jest szkieletem zgodnym z D01.
- Zależności odtwarzane z `packages.lock.json`, `package-lock.json` i lokalnego manifestu `dotnet-ef`. Bezpośrednia zależność [System.Security.Cryptography.Pkcs 10.0.13](https://www.nuget.org/packages/System.Security.Cryptography.Pkcs/10.0.13) aktualizuje wersję przechodnią używaną przez sterownik SQL.

Pełne polecenia odtworzenia i konfiguracja bez sekretów: [README.md](../README.md). Migracja jest wykonywana jawnie przed uruchomieniem; baza aplikacji nie otrzymuje danych testowych. Testy używają odrębnej losowej bazy, resetują dane przed każdym przypadkiem i usuwają własną bazę po zakończeniu. Kwoty korzystają z SQL `decimal(28,8)`, masa z `float`/`real`, a suma stanów w VM z `long`; szczegóły kontraktu opisano w README.

### Powiązanie stories z implementacją i testami

| Zakres | Implementacja | Weryfikacja programisty |
| --- | --- | --- |
| US-01-001, KA-01–05 | `WebStore.sln`, pliki projektów, `WebStore.Web`, `README.md`, Git main | Build, sprawdzenie referencji, czysta instalacja klienta, uruchomienie serwera i przeglądarka; identyfikacja K1 powyżej. |
| US-01-002, KA-01–04 | `WebStore.Model/DataModels/Users.cs`, `ApplicationDbContext` | `DatabaseTests`, relacje klientów i dostawców także w `ProductTests`, `OrderTests`, `StoreTests`. |
| US-01-003, KA-01–07 | `ProductService`, `StoreMapper`, `CatalogVm`, `Catalog.cs` | `ProductTests`, wspólne testy kontraktu: dane, aktualizacja, ID, walidacja i relacje. |
| US-01-004, KA-01–06 | `ProductService`, `StoreMapper` | `ProductTests`: dokładnie dwa produkty, filtrowanie, sumy 2 + 3 i brak stanów; `ServiceContractTests`: brak wyników i predykat null. |
| US-01-005, KA-01–07 | `AddressService`, `LocationsVm`, model i DAL | `AddressTests`: pola, właściciel, wskazania, ich czyszczenie, zmiana właściciela i atomowość; wspólny kontrakt. |
| US-01-006, KA-01–05 | `StoreService`, `ServiceRegistration`, model i VM | `StoreTests`: wiele adresów i pracowników, przeniesienie, brak ID i zły typ; utworzenie przez DI w fixture i wspólny kontrakt. |
| US-01-007, KA-01–08 | `OrderService`, `SalesVm`, `OrderProduct` | `OrderTests`: pozycje, daty, kwoty przy zapisie i edycji, brak zmian stanów, blokada edycji na fakturze; wspólny kontrakt. |
| US-01-008, KA-01–07 | `InvoiceService`, `SalesVm`, indeks numeru faktury | `InvoiceTests`: 35 + 20 = 55, klient, unikalność, zmiana zestawu i jego zachowanie po błędzie; wspólny kontrakt. |
| US-01-009, KA-01–06 | `WebStore.DAL/EF`, `WebStore.DAL/Migrations` | Rzeczywista migracja LocalDB, `DatabaseTests`: schemat, FK, TPH, nowy kontekst, ograniczenia danych; trwałe relacje również w testach usług. |
| US-01-010, KA-01–06 | `WebStore.Tests`, fixture `SqlDatabase` i `ServiceTest` | 81 testów wykonanych na SQL Server, bez pominięć; dowód TRX poniżej. |
| WU-01–07 | Pięć interfejsów i usług, `BaseService`, `StoreMapper` | `ServiceContractTests` wykonuje wspólne scenariusze dla każdej usługi; testy domenowe obejmują operacje powiązań. Helper `Rejected` sprawdza wyjątek, log, identyczny stan całej domeny w nowym kontekście oraz brak późniejszego zapisu odrzuconych zmian. |

### Wykonane kontrole — 2026-10-24

| Polecenie / scenariusz | Rzeczywisty wynik |
| --- | --- |
| `dotnet restore --locked-mode` i `dotnet tool restore` | Powodzenie; wersje zależności i narzędzia migracji odtworzone. |
| `dotnet build --no-restore` po restore | Wszystkie sześć projektów zbudowane; 0 błędów, 0 ostrzeżeń. |
| `dotnet ef database update --project WebStore.DAL --startup-project WebStore.DAL --no-build` | `InitialCreate` zastosowana do pustej bazy SQL Server LocalDB, wynik `Done.`. |
| `dotnet test --no-build --no-restore --logger 'trx;LogFileName=sprint-01.trx' --results-directory TestResults` | **81 zaliczonych, 0 niepowodzeń, 0 pominiętych**, czas testów 12 s. [Raport TRX](dowody/sprint-01-tests.trx). |
| Scenariusz błędu SQL przy edycji faktury | Test wymusza wyjątek podczas zapisu; potwierdza rollback danych i relacji oraz dalszą poprawną pracę tego samego kontekstu. |
| `npm.cmd --prefix WebStore.Web/ClientApp ci` | Czysta instalacja z lockfile zakończona sukcesem; audyt zgłosił 0 podatności. |
| `npm.cmd --prefix WebStore.Web/ClientApp run build` | Vite zbudował React do `WebStore.Web/wwwroot`. |
| `dotnet run --project WebStore.Web --no-build --no-restore` | Host nasłuchuje na `http://localhost:5100`. |
| Przeglądarka pod adresem hosta | Wyrenderowana strona React, tytuł „WebStore · ZPI”, polski nagłówek i opis; [zrzut ekranu](dowody/sprint-01-web.png). |
| `git diff --cached --check` | Brak błędów białych znaków przed commitem K1. |

**Znane usterki i blokady:** brak w sprawdzonym zakresie. To wyniki testów własnych programisty; nie stanowią akceptacji lidera ani werdyktu recenzenta. Recenzja nie została rozpoczęta.

## Jak rozpocząć / jak kontynuować

Aby uruchomić aplikację, wykonaj polecenia z [README.md](../README.md). Lider nie musi tworzyć żadnego pliku, folderu ani gałęzi w repozytorium.

Aby przejść do recenzji, po obejrzeniu K1 lider może napisać: „Akceptuję K1 (commit 8108edc) dla sprintu 01”. To wzór kolejnej decyzji, nie zapis akceptacji kodu. Agent zapisze faktyczną decyzję i dopiero wtedy rozpocznie rolę recenzenta. Uwagi do K1 uruchamiają etap poprawek.
