# ZPI — zasady projektu i sekwencyjna praca agentów

## Cel i język

Celem projektu ZPI jest dostarczenie aplikacji w architekturze klient–serwer. Na początku każdego sprintu w folderze `wymagania funkcjonalne/` pojawia się dokument PDF z wymaganiami danej iteracji.

Projekt jest prowadzony po polsku: rozmowy, pytania, user stories, kryteria akceptacji, opisy zmian i recenzje. Nazwy API, bibliotek i identyfikatorów kodu mogą zachowywać konwencję używanej technologii.

Preferowaną wersją platformy jest .NET 10. Zgodnie z decyzją lidera starszy PDF należy dostosować do tej wersji, jeśli jest to możliwe. Gdy rzeczywista przeszkoda uniemożliwia takie rozwiązanie, można dobrać wersję ułatwiającą realizację celu; przyczynę i wybór należy zapisać w karcie sprintu. Sam przykład użycia starszej wersji w PDF nie jest powodem do obniżenia wersji.

Ten plik jest głównym źródłem ustaleń organizacyjnych. Szczegóły ról znajdują się w trzech plikach:

| Rola | Instrukcja | Odpowiedzialność |
| --- | --- | --- |
| Analityk | [analityk.md](analityk.md) | Analiza PDF, wyjaśnienie niejasności, user stories i kryteria akceptacji. |
| Programista fullstack | [programista-fullstack.md](programista-fullstack.md) | Implementacja zatwierdzonego zakresu po stronie klienta i serwera, integracja i testy. |
| Recenzent | [recenzent.md](recenzent.md) | Weryfikacja funkcjonalności po akceptacji kodu przez lidera. |

## Zasada sekwencyjności

W danej chwili pracuje tylko jedna rola nad jednym aktywnym sprintem. Nie uruchamiaj ról równolegle ani nie deleguj pracy równoległym podagentom. Minimalne środowisko opiera się na kolejnych etapach pracy w czacie i przekazaniu ich wyników w pliku sprintu; nie wymaga dodatkowego orkiestratora ani integracji API.

Przed rozpoczęciem etapu przeczytaj ten plik, instrukcję właściwej roli i kartę bieżącego sprintu. Sprawdź etap, blokady oraz zakres i aktualność akceptacji. Na początku odpowiedzi wskaż aktywną rolę. Samo pojawienie się PDF nie uruchamia sprintu; rozpocznij po poleceniu lidera. Przy wznowieniu kontynuuj zapisany etap, zamiast tworzyć kolejną kartę tego samego sprintu.

Polecenie zmiany instrukcji lub organizacji projektu nie rozpoczyna realizacji wymagań aplikacji i nie stanowi akceptacji stories ani kodu.

## Przebieg sprintu

`Analityk → lider akceptuje stories → programista fullstack → lider akceptuje kod → recenzent`

| Etap w karcie sprintu | Działanie | Warunek przejścia |
| --- | --- | --- |
| `ANALIZA` | Analityk czyta wymagania, zadaje pytania i przygotowuje stories. | Jednoznaczny zakres, opis i mierzalne kryteria dla każdej story; przekazanie liderowi do akceptacji. |
| `CZEKA_NA_AKCEPTACJE_STORIES` | Agent przedstawia liderowi gotowy zakres i czeka. | Jednoznaczna akceptacja wskazanych stories i ich rewizji w czacie. |
| `IMPLEMENTACJA` | Programista realizuje zaakceptowany zakres i sprawdza rozwiązanie. | Gotowy kod, wyniki testów i opis wersji przedstawione liderowi. |
| `CZEKA_NA_AKCEPTACJE_KODU` | Agent czeka na decyzję lidera dotyczącą przedstawionej wersji kodu. | Jednoznaczna akceptacja tej wersji kodu w czacie. |
| `RECENZJA` | Recenzent weryfikuje zaakceptowaną wersję względem kryteriów. | Wynik pozytywny: `ZAKONCZONY`; usterki: `POPRAWKI`; brak możliwości oceny: blokada w bieżącym etapie. |
| `POPRAWKI` | Programista usuwa wskazane usterki w zatwierdzonym zakresie i ponawia testy. | Nowa rewizja kodu trafia do `CZEKA_NA_AKCEPTACJE_KODU`, a po akceptacji do ponownej recenzji. |
| `ZAKONCZONY` | Zapisany jest końcowy wynik i uporządkowane materiały sprintu. | Każde kryterium zatwierdzonego zakresu jest zweryfikowane pozytywnie; brak otwartych usterek i blokad tego zakresu. |

Uwagi lidera do stories cofają pracę do `ANALIZA`; uwagi do kodu trafiają do `POPRAWKI`. Nie pomijaj żadnej z dwóch akceptacji. Testy własne programisty są częścią implementacji; nie zastępują recenzji. Po zapisaniu wymaganej akceptacji kontynuuj kolejny etap bez ponownego proszenia o tę samą zgodę, chyba że lider poleci poczekać.

## Niejasności — zawsze pytaj

Jeżeli wymaganie, dokument źródłowy, kryterium, zakres sprintu, decyzja lidera lub warunek odbioru jest niejednoznaczny, **ZAWSZE zapytaj lidera**. Nie wybieraj domyślnej interpretacji i nie uzupełniaj brakującego wymagania własnym założeniem.

W pytaniu wskaż źródło niejasności, możliwe interpretacje i wpływ odpowiedzi na zakres lub weryfikację. Zapisz pytanie i zależną od niego pracę w karcie sprintu. Wstrzymaj tę pracę do odpowiedzi; możesz kontynuować niezależną, jednoznaczną część bieżącego etapu. Nie przechodź do kolejnej roli z nierozstrzygniętą niejasnością dotyczącą przekazywanego zakresu.

Jeżeli lider musi wykonać ręczną czynność w repozytorium, podaj konkretnie co zrobić, gdzie oraz jak dokładnie nazwać plik, folder lub gałąź. Odróżniaj gałąź Git od pliku i folderu. Czynności możliwe do wykonania przez agenta w zatwierdzonym etapie wykonuj samodzielnie, zamiast przerzucać je na lidera.

Dotyczy to również nieczytelnego PDF, sprzecznych dokumentów, niepewnego przypisania PDF do sprintu oraz brakujących wymagań technicznych wpływających na działanie lub odbiór aplikacji. Jednoznaczne decyzje implementacyjne w zatwierdzonym zakresie podejmuje programista. Jeśli wymagania pozostawiają istotny wybór produktu nierozstrzygnięty, rozstrzyga go lider.

Po odpowiedzi zastąp otwarte pytanie krótkim zapisem ustalenia i jego źródła. Zachowaj tylko informacje potrzebne do realizacji lub odbioru, bez transkrypcji całej rozmowy.

## Akceptacje lidera

Lider akceptuje **wprost w czacie**. Agent zapisuje decyzję i jej zakres w karcie sprintu. Nie wolno agentowi udzielać akceptacji w imieniu lidera ani uznawać braku odpowiedzi za zgodę. Odpowiedź na pytanie analityka, wynik testów lub ogólne polecenie „kontynuuj” nie zastępują jednoznacznej akceptacji konkretnego materiału.

Zapis akceptacji zawiera: rodzaj (`STORIES` albo `KOD`), datę i godzinę ze strefą `Europe/Warsaw`, lidera wskazanego w rozmowie, krótki cytat decyzji, zakres identyfikatorów stories i rewizję zaakceptowanego materiału. Nie wymyślaj danych lidera ani dokładniejszego czasu niż dostępny; gdy nazwisko nie jest znane, wystarcza „lider projektu — użytkownik czatu”.

Stories oznaczaj kolejnymi rewizjami `US1`, `US2`, a wersje kodu `K1`, `K2`. Przed pytaniem o akceptację pokaż liderowi konkretną rewizję, zakres i miejsce jej przeglądu. Wersję kodu powiąż z commitem i stanem bez późniejszych zmian w zakresie odbioru; bez Git użyj listy plików implementacji i testów z sumami SHA-256. Akceptacja obejmuje dokładnie przedstawioną zawartość.

Przy częściowej akceptacji zapisz dokładnie zaakceptowane stories. Jeżeli dalszy zakres sprintu nie wynika jasno z decyzji, dopytaj przed implementacją. Pozostałych stories nie realizuj i nie oznaczaj jako zaakceptowanych. Nie prowadź równocześnie analizy pozostałych stories i implementacji zatwierdzonych.

Zmiana wymagania lub kryterium po akceptacji wymaga ponownej analizy i akceptacji stories w zmienionym zakresie oraz ponownego sprawdzenia zgodności kodu. Każda zmiana kodu lub testów po akceptacji kodu wymaga nowej rewizji i ponownej akceptacji przed recenzją. Wcześniejszą decyzję oznacz jako zastąpioną dla zmienionego zakresu; nie przedstawiaj jej jako aktualnej i nie usuwaj dowodu jej udzielenia.

## Jedna karta na sprint

Przy rzeczywistym rozpoczęciu analizy utwórz `sprinty/sprint-NN.md`, gdzie `NN` jest numerem sprintu uzgodnionym z liderem. Nie twórz pustych kart ani przykładowych stories na etapie konfiguracji środowiska. Jeśli nie wiadomo, który sprint jest aktywny, zapytaj.

Karta jest wspólnym miejscem przekazania pracy między rolami. Zawiera:

1. **Stan:** numer, cel i zakres sprintu, etap, aktualna rola lub oczekiwanie na lidera, bieżące rewizje stories i kodu, blokady i kolejny krok.
2. **Źródła i ustalenia:** ścieżki PDF, ich sumy SHA-256, odwołania do stron lub punktów oraz wiążące odpowiedzi lidera. Zmieniony PDF wymaga sprawdzenia wpływu zmiany na stories i akceptacje.
3. **User stories:** identyfikator, tytuł, opis, źródło, kryteria akceptacji i zależności zgodnie z `analityk.md`.
4. **Pytania otwarte:** konkretne nierozstrzygnięte pytania i wskazanie blokowanej pracy; jeśli brak, wpisz „Brak”.
5. **Akceptacje:** decyzje lidera, ich zakres, rewizje oraz aktualność.
6. **Implementacja:** powiązanie stories z kodem, opis rewizji, sposób uruchomienia, potrzebna konfiguracja bez sekretów i rzeczywiste wyniki testów zgodnie z `programista-fullstack.md`.
7. **Recenzja:** wynik każdego kryterium, dowody, usterki i końcowy werdykt zgodnie z `recenzent.md`.

Sekcje dodawaj, gdy pojawia się ich treść. Brak wyniku przyszłego etapu nie oznacza wykonania ani akceptacji. Następna rola czyta poprzednie wyniki z tej samej karty; nie tworzy osobnego backlogu ani konkurencyjnego raportu sprintu.

## Porządek między iteracjami

Aktualizuj dokumenty w miejscu. Przed zakończeniem etapu usuń własne pliki tymczasowe, ekstrakcje PDF, próbne raporty i materiały redakcyjne, które nie są potrzebne jako dowód. Nie pozostawiaj plików typu `final2`, `nowy`, `kopia`, roboczych duplikatów, komentarzy do redakcji ani nieuzupełnionych placeholderów w przekazywanej treści.

Nie usuwaj oryginalnych PDF, kodu, testów, ustaleń, dowodów akceptacji, potrzebnych dowodów weryfikacji ani ukończonych kart sprintów. Nie usuwaj cudzych lub nieznanych plików pod pozorem sprzątania. Rozstrzygnięte pytania zastępuj ustaleniami, a nieaktualne opisy bieżącego stanu aktualizuj. Rzeczywiste otwarte pytania i usterki pozostają widoczne do rozwiązania.

Nowy sprint otrzymuje nową kartę i własne akceptacje. Zachowaj ukończone karty jako zapis wyników poprzednich iteracji; nie kopiuj ich roboczych uwag do nowego sprintu. Zmiany zasad organizacyjnych zapisuj w tym pliku i utrzymuj zgodność instrukcji ról.

## Jak rozpocząć

Po dodaniu PDF lider wskazuje numer sprintu i dokument do analizy. Przykładowe polecenie: „Rozpocznij jako analityk sprint 01 na podstawie `wymagania funkcjonalne/ZPI_N_Lab1.pdf`”. To przykład uruchomienia, nie zapis rozpoczęcia ani akceptacja wymagań.

Po przygotowaniu stories lider akceptuje podaną rewizję i zakres w czacie. Po implementacji tak samo akceptuje konkretną rewizję kodu. Recenzent kończy proces weryfikacją. Samo utworzenie tych instrukcji nie rozpoczyna sprintu ani nie ustala stosu technologicznego aplikacji.
