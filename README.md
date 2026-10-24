# WebStore — ZPI

Sprint 01 dostarcza model sklepu, bazę SQL Server, pięć usług biznesowych, testy i uruchamialny szkielet ASP.NET Core + React. Zakres i kryteria odbioru znajdują się w [karcie sprintu 01](sprinty/sprint-01.md). Strona pokazuje zapowiedź sklepu; ekrany zakupów i biznesowe API HTTP należą do przyszłych iteracji.

## Wymagania

- Windows i SQL Server LocalDB, instancja `MSSQLLocalDB`.
- .NET SDK 10 (sprawdzone: `10.0.401`). `global.json` dopuszcza aktualne wydania SDK 10.
- Node.js co najmniej 22.12 i npm (sprawdzone: Node `24.21.0`).
- Git do pobrania projektu. Rozszerzenia VS Code nie są wymagane.

## Pierwsze uruchomienie

Otwórz PowerShell w katalogu repozytorium, zawierającym `WebStore.sln`. Przy pierwszym pobraniu z GitHuba:

```powershell
git clone https://github.com/Fecasu/ZPI.git
cd ZPI
```

Przygotuj zależności i klienta:

```powershell
dotnet restore --locked-mode
dotnet tool restore
npm.cmd --prefix WebStore.Web/ClientApp ci
npm.cmd --prefix WebStore.Web/ClientApp run build
dotnet build --no-restore
```

Przygotuj bazę i uruchom aplikację:

```powershell
SqlLocalDB start MSSQLLocalDB
dotnet ef database update --project WebStore.DAL --startup-project WebStore.DAL --no-build
dotnet run --project WebStore.Web --no-build --no-restore
```

Otwórz [localhost:5100](http://localhost:5100). Zatrzymanie serwera: `Ctrl+C` w terminalu. Kolejne uruchomienie wymaga tylko ostatniego polecenia, o ile źródła się nie zmieniły. Po zmianie React ponów `npm.cmd --prefix WebStore.Web/ClientApp run build`; po zmianie C# ponów `dotnet build`.

Domyślne połączenie korzysta z uwierzytelniania Windows i bazy `ZpiWebStore`. Migracja tworzy schemat; baza aplikacji nie jest wypełniana fikcyjnymi produktami. Dane demonstracyjne istnieją wyłącznie w testach. Uruchomienie serwera nie wykonuje automatycznych migracji.

Opcjonalna konfiguracja innego połączenia, odczytywana zarówno przez host, jak i narzędzie migracji:

```powershell
$env:ConnectionStrings__WebStore = 'Server=(localdb)\MSSQLLocalDB;Database=ZpiWebStore;Trusted_Connection=True;TrustServerCertificate=True'
```

Nie zapisuj haseł w repozytorium. Domyślna konfiguracja dotyczy lokalnego środowiska programistycznego.

## Testy

```powershell
dotnet test
```

Testy xUnit korzystają z rzeczywistego SQL Server LocalDB. Każde uruchomienie zestawu tworzy nową bazę `ZpiWebStoreTests_<losowy identyfikator>`, stosuje migrację, odtwarza dane przed każdym testem i usuwa wyłącznie własną bazę po zakończeniu. Baza `ZpiWebStore` nie jest czyszczona przez testy. Brak LocalDB powoduje jawny błąd, a nie pominięcie testów.

Zapis raportu do lokalnego, ignorowanego przez Git katalogu:

```powershell
dotnet test --logger 'trx;LogFileName=sprint-01.trx' --results-directory TestResults
```

Zestaw obejmuje pięć usług, walidację i logowanie błędów, brak częściowych zapisów, aktualizacje powiązań, migrację, klucze obce i dziedziczenie TPH. Każdy test odrzuconej operacji porównuje dane przed i po przez osobny kontekst. Osobny scenariusz wymusza błąd SQL podczas aktualizacji faktury i sprawdza wycofanie transakcji.

## Struktura i kontrakty

| Projekt | Odpowiedzialność |
| --- | --- |
| `WebStore.Model` | Encje sklepu i użytkownicy dziedziczący po `IdentityUser<int>`. |
| `WebStore.DAL` | `ApplicationDbContext`, konfiguracja SQL Server i migracja `InitialCreate`. |
| `WebStore.ViewModels` | Wejściowe i wynikowe VM. |
| `WebStore.Services` | Interfejsy, implementacje pięciu usług, `StoreMapper` i rejestracja DI. |
| `WebStore.Web` | Host ASP.NET Core i klient React w `ClientApp`. Vite buduje pliki do `wwwroot`. |
| `WebStore.Tests` | Testy usług i bazy, izolowane dane testowe. |

Usługi rejestruje `services.AddWebStore(connectionString)`. Pobieraj interfejsy `IProductService`, `IAddressService`, `IStoreService`, `IOrderService`, `IInvoiceService` z zakresu DI. Każda usługa ma metody `AddOrUpdate…`, `Get…` i `Get…s`; pojedynczy odczyt wymaga predykatu `Expression<Func<TEntity, bool>>`, a filtr listy jest opcjonalny. Kolejność wyników nie jest gwarantowana.

- Brak ID albo `0` dodaje; dodatnie ID aktualizuje istniejący rekord.
- `ArgumentNullException`: brak VM lub wymaganego predykatu.
- `ArgumentException`: błędne dane, ujemne ID operacji zapisu, naruszenie reguły biznesowej lub niewłaściwy typ użytkownika.
- `KeyNotFoundException`: nieistniejący rekord albo wymagane powiązanie.
- Brak wyniku odczytu oznacza `null` albo pustą kolekcję.
- `AddressService.SetCustomerAddresses` ustawia lub czyści oba wskazania klienta; `GetCustomerAddresses` je odczytuje.
- `StoreService.AssignEmployee` przypisuje istniejącego pracownika lub przenosi go między sklepami.
- `InvoiceService.AddOrUpdateInvoice` zastępuje zestaw zamówień i zwalnia usunięte powiązania.

Każdy zapis jest odrębną transakcją `Serializable`, obejmującą walidację powiązań i zapis. Kontekst przekazany usłudze nie może zawierać niezapisanych zmian innych operacji. Usługa zapisuje własne zmiany oraz czyści śledzenie po sukcesie lub błędzie; po błędzie SQL transakcja jest wycofywana. Nie używaj jednego zakresu DI równolegle. Błędy infrastruktury są logowane i przekazywane wywołującemu.

Mapowanie jest jawne i typowane w `StoreMapper`, bez zależności od historycznej wersji biblioteki z PDF. Kwoty są przechowywane jako SQL `decimal(28,8)`, masa jako `real` zgodnie z modelem `float`; waluta to PLN, jednostka masy to kg. Suma stanów w VM używa `long`, aby sumowanie wielu rekordów `int` nie przepełniało wyniku. Unikalny indeks skrótu numeru faktury pozwala uniknąć narzucania biznesowego limitu długości numeru.

## Praca sekwencyjna

Zasady znajdują się w [codex.md](codex.md), a instrukcje ról w plikach `analityk.md`, `programista-fullstack.md` i `recenzent.md`. Przejście do programisty wymaga akceptacji stories; przejście do recenzenta — osobnej akceptacji konkretnej rewizji kodu przez lidera w czacie.
