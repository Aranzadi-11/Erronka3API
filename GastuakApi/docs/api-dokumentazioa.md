# Jatetxea APIaren dokumentazioa

## Sarrera

`GastuakApi` jatetxe baten barne kudeaketarako APIa da. Honako arloak kudeatzen ditu:

- Inbentarioko produktuak eta stock mugimenduak.
- Platerak, kategoriak eta plater bakoitzaren osagaiak.
- Mahaiak eta erreserbak.
- Langileak, rolak eta login eragiketa.
- Zerbitzuak, eskaerak, zerbitzu xehetasunak eta kaxa totala.
- Odoo datu-basetik deskontu kodeak balidatzea eta aplikatzea.

APIaren base ruta garapenean normalean honelakoa da:

```text
https://localhost:{port}/api
```

Swagger dokumentazioa garapen ingurunean:

```text
https://localhost:{port}/swagger
```

## Erantzun ohikoak

| Kodea | Esanahia | Noiz gertatzen da |
| --- | --- | --- |
| `200 OK` | Eragiketa ondo egin da | Datuak lortzean, sortzean, eguneratzean edo ezabatzean. |
| `400 Bad Request` | Eskaera ez da zuzena | Parametro bat falta denean, formatua okerra denean edo negozio arau bat betetzen ez denean. |
| `401 Unauthorized` | Login datuak okerrak dira | `Langileak/login` endpoint-ean erabiltzailea edo pasahitza okerra denean. |
| `404 Not Found` | Ez da aurkitu | URLko identifikatzailearekin elementurik ez dagoenean. |

Errore sinple gehienek egitura hau erabiltzen dute:

```json
{
  "mezua": "Ez da aurkitu"
}
```

## Controller-ak

### InbentarioaController

Base ruta: `api/Inbentarioa`

Inbentarioan dauden produktuak kudeatzen ditu. Produktu bakoitzak izena, deskribapena, kantitatea, neurri unitatea, stock minimoa eta azken eguneratze data ditu.

| Metodoa | Ruta | Zertarako balio du | Sarrera | Itzulera |
| --- | --- | --- | --- | --- |
| `GET` | `/api/Inbentarioa` | Inbentarioko elementu guztiak lortzeko. | Ez du body-rik. | `InbentarioaDto[]` |
| `GET` | `/api/Inbentarioa/{id}` | Elementu zehatz bat lortzeko. | `id` path parametroa. | `InbentarioaDto` edo `404`. |
| `POST` | `/api/Inbentarioa` | Elementu berria sortzeko. | `InbentarioaSortuDto` | `{ mezua, id }` |
| `PUT` | `/api/Inbentarioa/{id}` | Elementuaren datu guztiak ordezkatzeko. | `InbentarioaSortuDto` | `{ mezua }` edo `404`. |
| `PATCH` | `/api/Inbentarioa/{id}` | Elementuaren datu batzuk bakarrik aldatzeko. | `InbentarioaPatchDto` | `{ mezua }` edo `404`. |
| `DELETE` | `/api/Inbentarioa/{id}` | Elementua ezabatzeko. | `id` path parametroa. | `{ mezua }` edo `404`. |
| `PATCH` | `/api/Inbentarioa/{id}/kantitatea` | Kantitatea gehitzeko edo kentzeko. | `KantitateaAldatuDto` | `EragiketaEmaitzaDto` edo `400`. |

`PATCH /kantitatea` endpoint-ean `Aldaketa` positiboa bada stocka gehitzen da, eta negatiboa bada stocka kentzen da. Kantitatea ezin da 0 baino txikiagoa izan.

### PlaterakController

Base ruta: `api/Platerak`

Kartako platerak kudeatzen ditu. Platera kategoria batekin lotu daiteke, prezioa du eta `Erabilgarri` eremuaren bidez aktibo edo ez-aktibo markatzen da.

| Metodoa | Ruta | Zertarako balio du | Sarrera | Itzulera |
| --- | --- | --- | --- | --- |
| `GET` | `/api/Platerak` | Plater guztiak lortzeko. | Ez du body-rik. | `PlaterakDto[]` |
| `GET` | `/api/Platerak/{id}` | Plater zehatz bat lortzeko. | `id` path parametroa. | `PlaterakDto` edo `404`. |
| `POST` | `/api/Platerak` | Plater berria sortzeko. | `PlaterakSortuDto` | `{ mezua, id }` |
| `PUT` | `/api/Platerak/{id}` | Plateraren datu guztiak eguneratzeko. | `PlaterakSortuDto` | `{ mezua }` edo `404`. |
| `PATCH` | `/api/Platerak/{id}` | Plateraren datu batzuk eguneratzeko. | `PlaterakPatchDto` | `{ mezua }` edo `404`. |
| `DELETE` | `/api/Platerak/{id}` | Platera ezabatzeko. | `id` path parametroa. | `{ mezua }` edo `404`. |
| `GET` | `/api/Platerak/disponibilitatea` | Plater bakoitza zenbat unitate presta daitekeen kalkulatzeko. | Ez du body-rik. | `PlateraDisponibilitateaDto[]` |

Disponibilitatea kalkulatzeko APIak plateraren osagaiak eta inbentarioko stocka erabiltzen ditu. Platera `Erabilgarri != "Bai"` bada edo errezetarik ez badu, presta daitezkeen unitateak `0` dira.

### PlaterenOsagaiakController

Base ruta: `api/PlaterenOsagaiak`

Plater baten errezeta definitzen du. Lerro bakoitzak plater bat eta inbentarioko osagai bat lotzen ditu, eta plater unitate bakoitzerako behar den kantitatea adierazten du.

| Metodoa | Ruta | Zertarako balio du | Sarrera | Itzulera |
| --- | --- | --- | --- | --- |
| `GET` | `/api/PlaterenOsagaiak` | Plater-osagai lotura guztiak lortzeko. | Ez du body-rik. | `PlaterenOsagaiak[]` |
| `GET` | `/api/PlaterenOsagaiak/{plateraId}/{inbentarioaId}` | Lotura zehatz bat lortzeko. | `plateraId`, `inbentarioaId`. | `PlaterenOsagaiak` edo `404`. |
| `POST` | `/api/PlaterenOsagaiak` | Errezeta lerro berri bat sortzeko. | `PlaterenOsagaiakDto` eremuen baliokidea. | `{ mezua, platareaId, inbentarioaId }` |
| `PUT` | `/api/PlaterenOsagaiak/{plateraId}/{inbentarioaId}` | Lotura baten kantitatea eguneratzeko. | `Kantitatea` eremua. | `{ mezua }` edo `404`. |
| `DELETE` | `/api/PlaterenOsagaiak/{plateraId}/{inbentarioaId}` | Lotura ezabatzeko. | `plateraId`, `inbentarioaId`. | `{ mezua }` edo `404`. |

Endpoint hauek gako konposatua erabiltzen dute: `PlateraId` eta `InbentarioaId`.

### KategoriaController

Base ruta: `api/Kategoria`

Platerak sailkatzeko kategoriak kudeatzen ditu.

| Metodoa | Ruta | Zertarako balio du | Sarrera | Itzulera |
| --- | --- | --- | --- | --- |
| `GET` | `/api/Kategoria` | Kategoria guztiak lortzeko. | Ez du body-rik. | `KategoriaDto[]` |
| `GET` | `/api/Kategoria/{id}` | Kategoria zehatz bat lortzeko. | `id` path parametroa. | `KategoriaDto` edo `404`. |
| `POST` | `/api/Kategoria` | Kategoria berria sortzeko. | `KategoriaSortuDto` | `{ mezua, id }` |
| `PUT` | `/api/Kategoria/{id}` | Kategoria eguneratzeko. | `KategoriaSortuDto` | `{ mezua }` edo `404`. |
| `DELETE` | `/api/Kategoria/{id}` | Kategoria ezabatzeko. | `id` path parametroa. | `{ mezua }` edo `404`. |

### RolakController

Base ruta: `api/Rolak`

Langileen rolak kudeatzen ditu.

| Metodoa | Ruta | Zertarako balio du | Sarrera | Itzulera |
| --- | --- | --- | --- | --- |
| `GET` | `/api/Rolak` | Rol guztiak lortzeko. | Ez du body-rik. | `RolakDto[]` |
| `GET` | `/api/Rolak/{id}` | Rol zehatz bat lortzeko. | `id` path parametroa. | `RolakDto` edo `404`. |
| `POST` | `/api/Rolak` | Rol berria sortzeko. | `RolakSortuDto` | `{ mezua, id }` |
| `PUT` | `/api/Rolak/{id}` | Rola eguneratzeko. | `RolakSortuDto` | `{ mezua }` edo `404`. |
| `DELETE` | `/api/Rolak/{id}` | Rola ezabatzeko. | `id` path parametroa. | `{ mezua }` edo `404`. |

### LangileakController

Base ruta: `api/Langileak`

Langileak eta login eragiketa kudeatzen ditu. Langile bakoitza rol batekin lotu daiteke eta txat baimena izan dezake.

| Metodoa | Ruta | Zertarako balio du | Sarrera | Itzulera |
| --- | --- | --- | --- | --- |
| `GET` | `/api/Langileak` | Langile guztiak lortzeko. | Ez du body-rik. | `LangileakDto[]` |
| `GET` | `/api/Langileak/{id}` | Langile zehatz bat lortzeko. | `id` path parametroa. | `LangileakDto` edo `404`. |
| `POST` | `/api/Langileak/login` | Erabiltzailea eta pasahitza egiaztatzeko. | `LoginRequest` | `LangileakDto`, `400` edo `401`. |
| `POST` | `/api/Langileak` | Langile berria sortzeko. | `LangileakSortuDto` | `{ mezua, id }` |
| `PUT` | `/api/Langileak/{id}` | Langilea eguneratzeko. | `LangileakSortuDto` | `{ mezua }` edo `404`. |
| `DELETE` | `/api/Langileak/{id}` | Langilea ezabatzeko. | `id` path parametroa. | `{ mezua }` edo `404`. |

Login-ean `Erabiltzailea` maiuskula/minuskulak kontuan hartu gabe konparatzen da, eta `Pasahitza` testu gisa konparatzen da. `GetAll` eta `Get` endpoint-ek pasahitza itzultzen dute; login erantzunean, ordea, ez da pasahitza betetzen.

### MahaiakController

Base ruta: `api/Mahaiak`

Jatetxeko mahaiak kudeatzen ditu eta data-ordu batean libre dauden mahaiak kalkulatzen ditu.

| Metodoa | Ruta | Zertarako balio du | Sarrera | Itzulera |
| --- | --- | --- | --- | --- |
| `GET` | `/api/Mahaiak` | Mahai guztiak lortzeko. | Ez du body-rik. | `MahaiakDto[]` |
| `GET` | `/api/Mahaiak/{id}` | Mahai zehatz bat lortzeko. | `id` path parametroa. | `MahaiakDto` edo `404`. |
| `POST` | `/api/Mahaiak` | Mahai berria sortzeko. | `MahaiakSortuDto` | `{ mezua, id }` |
| `PUT` | `/api/Mahaiak/{id}` | Mahaiaren datuak eguneratzeko. | `MahaiakSortuDto` | `{ mezua }` edo `404`. |
| `DELETE` | `/api/Mahaiak/{id}` | Mahaia ezabatzeko. | `id` path parametroa. | `{ mezua }` edo `404`. |
| `GET` | `/api/Mahaiak/libre?dataOrdua=2026-03-16T14:00:00` | Data eta ordu zehatz batean libre dauden mahaiak lortzeko. | `dataOrdua` query parametroa. | `MahaiakDto[]` edo `400`. |

Oharra: `MahaiakRepository.Get` metodoak `MahaiaZbk` eremua erabiltzen du bilaketarako. Horregatik, egungo inplementazioan `id` parametroak praktikan mahai zenbakia ordezkatzen du.

### ErreserbakController

Base ruta: `api/Erreserbak`

Mahai erreserbak kudeatzen ditu. Erreserba bat ezabatzean, berarekin lotutako zerbitzuei `ErreserbaId = null` jartzen zaie lehenik.

| Metodoa | Ruta | Zertarako balio du | Sarrera | Itzulera |
| --- | --- | --- | --- | --- |
| `GET` | `/api/Erreserbak` | Erreserba guztiak lortzeko. | Ez du body-rik. | `ErreserbakDto[]` |
| `GET` | `/api/Erreserbak/{id}` | Erreserba zehatz bat lortzeko. | `id` path parametroa. | `ErreserbakDto` edo `404`. |
| `POST` | `/api/Erreserbak` | Erreserba berria sortzeko. | `ErreserbakSortuDto` | `{ mezua, id }` |
| `PUT` | `/api/Erreserbak/{id}` | Erreserba eguneratzeko. | `ErreserbakSortuDto` | `{ mezua }` edo `404`. |
| `DELETE` | `/api/Erreserbak/{id}` | Erreserba ezabatzeko. | `id` path parametroa. | `{ mezua }` edo `404`. |
| `GET` | `/api/Erreserbak/gaur` | Gaurko erreserbak lortzeko. | Ez du body-rik. | `ErreserbakDto[]` |
| `GET` | `/api/Erreserbak/etorkizunak` | Une honetatik aurrerako erreserbak lortzeko. | Ez du body-rik. | `ErreserbakDto[]` |
| `GET` | `/api/Erreserbak/bilatu?data=2026-03-16&ordua=14:00` | Data edo orduaren arabera bilatzeko. | `data` eta `ordua` query parametro aukerazkoak. | `ErreserbakDto[]` edo `400`. |

`bilatu` endpoint-aren portaera:

- `data` eta `ordua` hutsik: etorkizuneko erreserbak itzultzen ditu.
- `data` bakarrik: data horretako eta oraindik pasatu gabeko erreserbak.
- `ordua` bakarrik: gaurko datarekin ordu hori duten erreserbak.
- Biak batera: data eta ordu zehatz hori duten erreserbak.

### ZerbitzuakController

Base ruta: `api/Zerbitzuak`

Mahai edo erreserba bateko eskaera osoa kudeatzen du. Zerbitzuak langile bat, mahai bat, aukerazko erreserba bat, egoera eta guztirakoa ditu.

| Metodoa | Ruta | Zertarako balio du | Sarrera | Itzulera |
| --- | --- | --- | --- | --- |
| `GET` | `/api/Zerbitzuak` | Zerbitzu guztiak lortzeko. | Ez du body-rik. | `ZerbitzuakDto[]` |
| `GET` | `/api/Zerbitzuak/{id}` | Zerbitzu zehatz bat lortzeko. | `id` path parametroa. | `ZerbitzuakDto` edo `404`. |
| `POST` | `/api/Zerbitzuak` | Zerbitzu sinple bat sortzeko. | `ZerbitzuakSortuDto` | `{ mezua, id }` |
| `PUT` | `/api/Zerbitzuak/{id}` | Zerbitzua eguneratzeko. | `ZerbitzuakSortuDto` | `{ mezua }` edo `404`. |
| `DELETE` | `/api/Zerbitzuak/{id}` | Zerbitzua ezabatzeko. | `id` path parametroa. | `{ mezua }` edo `404`. |
| `POST` | `/api/Zerbitzuak/egin` | Erreserba baten eskaera sortu edo eguneratzeko, platerak eta stocka kontuan hartuta. | `ZerbitzuaEskariaDto` | `ZerbitzuaEmaitzaDto` |
| `GET` | `/api/Zerbitzuak/erreserba/{erreserbaId}/platerak` | Erreserba baten plater laburpena lortzeko. | `erreserbaId` path parametroa. | `{ plateraId, kantitatea, zerbitzatuta }[]` edo `404`. |
| `GET` | `/api/Zerbitzuak/gaur` | Gaurko zerbitzuak lortzeko. | Ez du body-rik. | `ZerbitzuakDto[]` |
| `GET` | `/api/Zerbitzuak/egunekoak` | Gaurko zerbitzuak lortzeko. | Ez du body-rik. | `ZerbitzuakDto[]` |
| `GET` | `/api/Zerbitzuak/erreserba/{erreserbaId}` | Erreserba bati lotutako zerbitzua lortzeko. | `erreserbaId` path parametroa. | `ZerbitzuakDto` edo `404`. |
| `GET` | `/api/Zerbitzuak/erreserba/{erreserbaId}/laburpena` | Erreserba bateko plateren laburpen aberastua lortzeko. | `erreserbaId` path parametroa. | `ZerbitzuLaburpenaDto[]` |
| `PATCH` | `/api/Zerbitzuak/{id}/egoera` | Zerbitzuaren egoera aldatzeko. | `ZerbitzuEgoeraPatchDto` | `{ mezua }`, `400` edo `404`. |

`POST /api/Zerbitzuak/egin` endpoint-ak negozio-logika berezia dauka:

- `ErreserbaId` horrekin zerbitzurik ez badago, zerbitzu berria sortzen du `Eskatuta` egoerarekin.
- Zerbitzua existitzen bada, bere plater lerroak eguneratzen ditu.
- Plater kopurua handitzean osagaiak inbentariotik kentzen ditu.
- Plater kopurua jaistean osagaiak itzultzen ditu, baldin eta lerroa oraindik zerbitzatuta ez badago.
- Lerroa jada zerbitzatuta badago, kantitatea ezin da aurreko balioaren azpitik jaitsi.
- Amaieran zerbitzuaren `Guztira` berriz kalkulatzen da.

Egoera balio erabilienak: `Itxaropean`, `Eskatuta`, `Prestatzen`, `Amaituta`, `Ordainduta`.

### ZerbitzuXehetasunakController

Base ruta: `api/ZerbitzuXehetasunak`

Zerbitzu bateko plater lerroak kudeatzen ditu. Lerro bakoitzak platera, kantitatea, prezio unitarioa eta zerbitzatuta dagoen ala ez gordetzen du.

| Metodoa | Ruta | Zertarako balio du | Sarrera | Itzulera |
| --- | --- | --- | --- | --- |
| `GET` | `/api/ZerbitzuXehetasunak` | Xehetasun guztiak lortzeko. | Ez du body-rik. | `ZerbitzuXehetasunakDto[]` |
| `GET` | `/api/ZerbitzuXehetasunak/{id}` | Xehetasun zehatz bat lortzeko. | `id` path parametroa. | `ZerbitzuXehetasunakDto` edo `404`. |
| `POST` | `/api/ZerbitzuXehetasunak` | Xehetasun berria sortzeko. | `ZerbitzuXehetasunakSortuDto` | `{ mezua, id }` |
| `PUT` | `/api/ZerbitzuXehetasunak/{id}` | Xehetasuna osorik eguneratzeko. | `ZerbitzuXehetasunakSortuDto` | `{ mezua }` edo `404`. |
| `DELETE` | `/api/ZerbitzuXehetasunak/{id}` | Xehetasuna ezabatzeko. | `id` path parametroa. | `{ mezua }` edo `404`. |
| `GET` | `/api/ZerbitzuXehetasunak/zerbitzua/{zerbitzuaId}` | Zerbitzu bateko xehetasun guztiak lortzeko. | `zerbitzuaId` path parametroa. | `ZerbitzuXehetasunakDto[]` |
| `PATCH` | `/api/ZerbitzuXehetasunak/{id}/zerbitzatuta` | Plater lerro bat zerbitzatuta edo ez-zerbitzatuta markatzeko. | `ZerbitzatutaPatchDto` | `EragiketaEmaitzaDto` edo `400`. |

`PATCH /zerbitzatuta` endpoint-ak stocka eguneratzen du. `Zerbitzatuta = true` jartzean osagaiak inbentariotik kentzen ditu; `false` jartzean osagaiak berriro gehitzen ditu.

### JatetxekoInfoController

Base ruta: `api/JatetxekoInfo`

Jatetxearen informazio orokorra kudeatzen du: izena, kaxa totala, helbidea eta telefono zenbakia.

| Metodoa | Ruta | Zertarako balio du | Sarrera | Itzulera |
| --- | --- | --- | --- | --- |
| `GET` | `/api/JatetxekoInfo` | Jatetxeko informazio guztiak lortzeko. | Ez du body-rik. | `JatetxekoInfoDto[]` |
| `GET` | `/api/JatetxekoInfo/{id}` | Informazio zehatz bat lortzeko. | `id` path parametroa. | `JatetxekoInfoDto` edo `404`. |
| `POST` | `/api/JatetxekoInfo` | Informazio erregistro berria sortzeko. | `JatetxekoInfoSortuDto` | `{ mezua, id }` |
| `PUT` | `/api/JatetxekoInfo/{id}` | Informazioa eguneratzeko. | `JatetxekoInfoSortuDto` | `{ mezua }` edo `404`. |
| `DELETE` | `/api/JatetxekoInfo/{id}` | Informazioa ezabatzeko. | `id` path parametroa. | `{ mezua }` edo `404`. |

`KaxaTotalaKalkulatu` background service-ak minutuero `Ordainduta` egoeran dauden zerbitzuen guztirakoa batu eta lehen jatetxe erregistroaren `KaxaTotal` eguneratzen du.

### DeskontuakController

Base ruta: `api/Deskontuak`

Odoo/PostgreSQL datu-baseko `jatetxeko_deskontuak` taulako deskontu kodeak balidatzen eta aplikatzen ditu.

| Metodoa | Ruta | Zertarako balio du | Sarrera | Itzulera |
| --- | --- | --- | --- | --- |
| `POST` | `/api/Deskontuak/validate` | Kode bat baliozkoa den egiaztatzeko. | `DeskuntuKodeaDto` | `DeskuntuEmaitzaDto` |
| `POST` | `/api/Deskontuak/apply` | Kodea balidatu eta erabilera kopurua handitzeko. | `DeskuntuKodeaDto` | `DeskuntuEmaitzaDto` |

Kodea normalizatu egiten da konparatu aurretik: zuriuneak, kontrol karaktereak eta formatu karaktereak kentzen dira, eta maiuskulaz konparatzen da. Balidazioak kontuan hartzen ditu `active`, `usage_limit`, `usage_count`, `valid_from` eta `valid_until`.

## DTO-ak

### Inbentarioa DTO-ak

`InbentarioaSortuDto` elementu berriak sortzeko eta `PUT` bidez eguneratzeko erabiltzen da.

| Eremua | Mota | Zertarako |
| --- | --- | --- |
| `Izena` | `string` | Produktuaren izena. |
| `Deskribapena` | `string?` | Produktuaren azalpena. |
| `Kantitatea` | `int` | Uneko stock kopurua. |
| `NeurriaUnitatea` | `string?` | Neurtzeko unitatea, adibidez kg, l edo unitateak. |
| `StockMinimoa` | `int` | Gutxieneko stock gomendatua. |

`InbentarioaPatchDto` eguneratze partzialetarako erabiltzen da. Eremu guztiak aukerazkoak dira, eta bidalitakoak bakarrik aldatzen dira.

`InbentarioaDto` erantzunetan erabiltzen da eta aurreko eremuez gain `Id` eta `AzkenEguneratzea` ditu.

### Platerak DTO-ak

`PlaterakSortuDto` plater berriak sortzeko eta osorik eguneratzeko erabiltzen da.

| Eremua | Mota | Zertarako |
| --- | --- | --- |
| `Izena` | `string` | Plateraren izena. |
| `Deskribapena` | `string?` | Plateraren azalpena. |
| `Prezioa` | `decimal` | Plateraren prezioa. |
| `KategoriaId` | `int?` | Platera zein kategoriatan dagoen. |
| `Erabilgarri` | `string` | Platera eskuragarri dagoen adierazteko, normalean `Bai` edo `Ez`. |
| `Irudia` | `string?` | Irudiaren URL edo bidea. |

`PlaterakPatchDto` eguneratze partzialetarako erabiltzen da. `PlaterakDto` erantzunetan erabiltzen da eta `Id` eta `SortzeData` ere baditu.

`PlateraDisponibilitateaDto` `GET /api/Platerak/disponibilitatea` endpoint-ean itzultzen da.

| Eremua | Mota | Zertarako |
| --- | --- | --- |
| `Id` | `int` | Plateraren identifikatzailea. |
| `Izena` | `string` | Plateraren izena. |
| `KategoriaId` | `int?` | Kategoriaren identifikatzailea. |
| `KategoriaIzena` | `string?` | Kategoriaren izena. |
| `Erabilgarri` | `string` | Plateraren erabilgarritasun marka. |
| `PrestatuDaitezkeenUnitateak` | `int` | Stockarekin prestatu daitezkeen unitate kopurua. |

### PlaterenOsagaiak DTO-a

`PlaterenOsagaiakDto` plater baten errezeta lerroa adierazteko erabiltzen da.

| Eremua | Mota | Zertarako |
| --- | --- | --- |
| `PlateraId` | `int` | Zein platerari dagokion. |
| `InbentarioaId` | `int` | Zein osagai erabiltzen duen. |
| `Kantitatea` | `decimal` | Plater unitate bakoitzeko behar den osagai kopurua. |

### Kategoria eta Rolak DTO-ak

`KategoriaDto` eta `RolakDto` erantzunetan erabiltzen dira; biek `Id` eta `Izena` dituzte.

`KategoriaSortuDto` eta `RolakSortuDto` sortzeko edo eguneratzeko erabiltzen dira; biek `Izena` bakarrik jasotzen dute.

### Langileak DTO-ak

`LangileakSortuDto` langileak sortzeko eta eguneratzeko erabiltzen da.

| Eremua | Mota | Zertarako |
| --- | --- | --- |
| `Izena` | `string` | Langilearen izen osoa. |
| `Erabiltzailea` | `string` | Login-erako erabiltzaile izena. |
| `Pasahitza` | `string` | Login-erako pasahitza. |
| `Aktibo` | `string` | Langilea aktibo dagoen, normalean `Bai` edo `Ez`. |
| `ErregistroData` | `DateTime?` | Langilea noiz erregistratu den. |
| `RolaId` | `int?` | Langilearen rola. |
| `TxatBaimena` | `bool` | Txata erabiltzeko baimena duen. |

`LangileakDto` erantzunetan erabiltzen da eta `Id` gehitzen du.

`LoginRequest` login endpoint-erako erabiltzen da.

| Eremua | Mota | Zertarako |
| --- | --- | --- |
| `Erabiltzailea` | `string` | Saioa hasteko erabiltzaile izena. |
| `Pasahitza` | `string` | Saioa hasteko pasahitza. |

### Mahaiak DTO-ak

`MahaiakSortuDto` mahaiak sortzeko eta eguneratzeko erabiltzen da.

| Eremua | Mota | Zertarako |
| --- | --- | --- |
| `MahaiaZbk` | `int` | Mahaiaren zenbakia. |
| `Edukiera` | `int` | Zenbat pertsonarentzako den. |
| `Egoera` | `string?` | Mahaiaren egoera, adibidez `Libre` edo `Okupatuta`. |

`MahaiakDto` erantzunetan erabiltzen da eta `Id` gehitzen du.

### Erreserbak DTO-ak

`ErreserbakSortuDto` erreserbak sortzeko eta eguneratzeko erabiltzen da.

| Eremua | Mota | Zertarako |
| --- | --- | --- |
| `MahaiaId` | `int` | Erreserbatutako mahaia. |
| `Izena` | `string` | Bezeroaren izena. |
| `Telefonoa` | `int` | Bezeroaren telefono zenbakia. |
| `ErreserbaData` | `DateTime?` | Erreserbaren data eta ordua. |
| `PertsonaKop` | `int?` | Pertsona kopurua. |
| `Egoera` | `string` | Erreserbaren egoera, lehenetsia `Itxaropean`. |
| `Oharrak` | `string?` | Erreserbaren ohar gehigarriak. |

`ErreserbakDto` erantzunetan erabiltzen da eta `Id` gehitzen du.

### Zerbitzuak DTO-ak

`ZerbitzuakSortuDto` zerbitzu sinpleak sortzeko eta eguneratzeko erabiltzen da.

| Eremua | Mota | Zertarako |
| --- | --- | --- |
| `LangileId` | `int` | Zerbitzua kudeatzen duen langilea. |
| `MahaiaId` | `int` | Zerbitzua zein mahairekin lotzen den. |
| `ErreserbaId` | `int?` | Aukerazko erreserba lotura. |
| `EskaeraData` | `DateTime?` | Eskaera noiz egin den. |
| `Egoera` | `string` | Zerbitzuaren egoera, lehenetsia `Itxaropean`. |
| `Guztira` | `decimal?` | Zerbitzuaren guztirako prezioa. |

`ZerbitzuakDto` erantzunetan erabiltzen da eta `Id` gehitzen du.

`ZerbitzuaEskariaDto` `POST /api/Zerbitzuak/egin` endpoint-erako erabiltzen da.

| Eremua | Mota | Zertarako |
| --- | --- | --- |
| `LangileId` | `int` | Eskaera egiten duen langilea. |
| `MahaiaId` | `int` | Eskaeraren mahaia. |
| `ErreserbaId` | `int` | Eskaerarekin lotutako erreserba. |
| `EskaeraData` | `DateTime` | Eskaera data. Egungo repositoryak zerbitzu berria sortzean `DateTime.Now` erabiltzen du. |
| `Platerak` | `List<PlateraEskariaDto>` | Eskatutako plateren zerrenda. |

`PlateraEskariaDto`:

| Eremua | Mota | Zertarako |
| --- | --- | --- |
| `PlateraId` | `int` | Eskatutako platera. |
| `Kantitatea` | `int` | Eskatutako kopurua. |

`ZerbitzuaEmaitzaDto` zerbitzua egitearen emaitza da.

| Eremua | Mota | Zertarako |
| --- | --- | --- |
| `Ondo` | `bool` | Eragiketa ondo joan den. |
| `ZerbitzuaId` | `int?` | Sortu edo eguneratutako zerbitzua. |
| `Erroreak` | `List<ZerbitzuErroreaDto>` | Plater mailako erroreak jasotzeko zerrenda. |

`ZerbitzuEgoeraPatchDto`:

| Eremua | Mota | Zertarako |
| --- | --- | --- |
| `Egoera` | `string` | Zerbitzuari jarri nahi zaion egoera berria. |

`ZerbitzuLaburpenaDto` `GET /api/Zerbitzuak/erreserba/{erreserbaId}/laburpena` endpoint-ean erabiltzen da.

| Eremua | Mota | Zertarako |
| --- | --- | --- |
| `ZerbitzuaId` | `int` | Zerbitzuaren identifikatzailea. |
| `ZerbitzuXehetasunaId` | `int` | Plater lerroaren identifikatzailea. |
| `PlateraId` | `int` | Plateraren identifikatzailea. |
| `PlateraIzena` | `string` | Plateraren izena. |
| `KategoriaId` | `int?` | Kategoriaren identifikatzailea. |
| `KategoriaIzena` | `string?` | Kategoriaren izena. |
| `Kantitatea` | `int` | Eskatutako kopurua. |
| `PrezioUnitarioa` | `decimal` | Unitateko prezioa. |
| `Zerbitzatuta` | `bool` | Platera zerbitzatuta dagoen. |

### ZerbitzuXehetasunak DTO-ak

`ZerbitzuXehetasunakSortuDto` zerbitzu bateko plater lerroak sortzeko eta eguneratzeko erabiltzen da.

| Eremua | Mota | Zertarako |
| --- | --- | --- |
| `ZerbitzuaId` | `int` | Zein zerbitzuri dagokion. |
| `PlateraId` | `int` | Zein plater den. |
| `Kantitatea` | `int` | Zenbat unitate eskatu diren. |
| `PrezioUnitarioa` | `decimal` | Unitateko prezioa eskaera unean. |
| `Zerbitzatuta` | `bool` | Platera zerbitzatuta dagoen. |

`ZerbitzuXehetasunakDto` erantzunetan erabiltzen da eta `Id` gehitzen du.

`ZerbitzatutaPatchDto`:

| Eremua | Mota | Zertarako |
| --- | --- | --- |
| `Zerbitzatuta` | `bool` | Lerroa zerbitzatuta edo ez-zerbitzatuta markatzeko. |

### Eragiketa DTO-ak

`KantitateaAldatuDto` `PATCH /api/Inbentarioa/{id}/kantitatea` endpoint-erako erabiltzen da.

| Eremua | Mota | Zertarako |
| --- | --- | --- |
| `Aldaketa` | `int` | Gehitu edo kendu nahi den kantitatea. Balio negatiboak stocka kentzen du. |

`EragiketaEmaitzaDto` stock edo egoera aldaketen emaitza da.

| Eremua | Mota | Zertarako |
| --- | --- | --- |
| `Ondo` | `bool` | Eragiketa ondo egin den. |
| `Mezua` | `string` | Emaitzaren azalpena. |
| `Id` | `int?` | Eragindako elementuaren identifikatzailea. |
| `KantitateBerria` | `int?` | Stock berria, aplikagarria denean. |

### JatetxekoInfo DTO-ak

`JatetxekoInfoSortuDto` jatetxearen informazioa sortzeko eta eguneratzeko erabiltzen da.

| Eremua | Mota | Zertarako |
| --- | --- | --- |
| `Izena` | `string` | Jatetxearen izena. |
| `KaxaTotal` | `decimal` | Ordaindutako zerbitzuen guztirakoa. |
| `Helbidea` | `string` | Jatetxearen helbidea. |
| `TelefonoZenbakia` | `int` | Jatetxearen telefonoa. |

`JatetxekoInfoDto` erantzunetan erabiltzen da eta `Id` gehitzen du.

### Deskontuak DTO-ak

`DeskuntuKodeaDto` deskontu kode bat jasotzeko erabiltzen da. Frontend edo kanpoko sistema ezberdinek izen ezberdinak bidali ditzaketenez, DTO honek hainbat alias onartzen ditu.

| Eremua | Mota | Zertarako |
| --- | --- | --- |
| `code` | `string?` | Kodearen izen estandarra ingelesez. |
| `kodea` | `string?` | Kodearen izena euskaraz. |
| `codigo` | `string?` | Kodearen izena gaztelaniaz. |
| `coupon_code` | `string?` | Kupoi kodea. |
| `discount_code` | `string?` | Deskontu kodea. |
| `name` | `string?` | Odoo edo beste sistemaren izen eremua. |
| `order_id` | `int?` | Eskaeraren identifikatzailea, oraingoz kodea ateratzeko ez da erabiltzen. |

`DeskuntuEmaitzaDto` deskontu balidazioaren emaitza da.

| Eremua | Mota | Zertarako |
| --- | --- | --- |
| `valid` | `bool` | Kodea baliozkoa den. |
| `message` | `string` | Balidazioaren mezua. |
| `percentage` | `double` | Deskontu portzentajea. |
| `code_id` | `int?` | Odoo datu-baseko kodearen identifikatzailea. |

## Adibide praktikoak

### Inbentarioko elementu bat sortu

```http
POST /api/Inbentarioa
Content-Type: application/json
```

```json
{
  "izena": "Tomatea",
  "deskribapena": "Tomate naturala",
  "kantitatea": 25,
  "neurriaUnitatea": "kg",
  "stockMinimoa": 5
}
```

Erantzuna:

```json
{
  "mezua": "Elementua sortuta",
  "id": 12
}
```

### Stocka murriztu

```http
PATCH /api/Inbentarioa/12/kantitatea
Content-Type: application/json
```

```json
{
  "aldaketa": -3
}
```

Erantzuna:

```json
{
  "ondo": true,
  "mezua": "Kantitatea eguneratuta",
  "id": 12,
  "kantitateBerria": 22
}
```

### Erreserba bat sortu

```http
POST /api/Erreserbak
Content-Type: application/json
```

```json
{
  "mahaiaId": 4,
  "izena": "Ane Etxeberria",
  "telefonoa": 600123123,
  "erreserbaData": "2026-03-16T14:00:00",
  "pertsonaKop": 4,
  "egoera": "Itxaropean",
  "oharrak": "Leiho ondoan"
}
```

### Erreserba baten eskaera egin

```http
POST /api/Zerbitzuak/egin
Content-Type: application/json
```

```json
{
  "langileId": 1,
  "mahaiaId": 4,
  "erreserbaId": 20,
  "eskaeraData": "2026-03-16T14:10:00",
  "platerak": [
    {
      "plateraId": 3,
      "kantitatea": 2
    },
    {
      "plateraId": 7,
      "kantitatea": 1
    }
  ]
}
```

Erantzuna:

```json
{
  "ondo": true,
  "zerbitzuaId": 31,
  "erroreak": []
}
```

### Zerbitzu baten egoera aldatu

```http
PATCH /api/Zerbitzuak/31/egoera
Content-Type: application/json
```

```json
{
  "egoera": "Ordainduta"
}
```

### Plater lerro bat zerbitzatuta markatu

```http
PATCH /api/ZerbitzuXehetasunak/45/zerbitzatuta
Content-Type: application/json
```

```json
{
  "zerbitzatuta": true
}
```

Endpoint honek stocka eguneratzen du, plater horren errezetako osagaiak kontsumituz.

### Libre dauden mahaiak bilatu

```http
GET /api/Mahaiak/libre?dataOrdua=2026-03-16T14:00:00
```

Erantzuna:

```json
[
  {
    "id": 2,
    "mahaiaZbk": 2,
    "edukiera": 4,
    "egoera": "Libre"
  }
]
```

### Deskontu kodea balidatu

```http
POST /api/Deskontuak/validate
Content-Type: application/json
```

```json
{
  "code": "UDA10"
}
```

Erantzuna:

```json
{
  "valid": true,
  "message": "OK",
  "percentage": 10,
  "code_id": 1
}
```

## Garatzaileentzako oharrak

- Swagger dokumentazioa `Program.cs` fitxategian controller multzoka banatuta dago.
- XML dokumentazioa aktibatuta dago `.csproj` fitxategian, eta Swaggerrek XML laburpenak irakurtzen ditu build output-ean fitxategia badago.
- `DeskontuakController` Odoo/PostgreSQL konexioarekin lan egiten du. Konexio katea `appsettings.json` fitxategiko `OdooPostgres` izenarekin irakurtzen da.
- `KaxaTotalaKalkulatu` background service-ak minutuero `Ordainduta` egoeran dauden zerbitzuak batzen ditu.
- `MahaiakRepository.Get` metodoaren portaera berezia da: `MahaiaZbk` erabiltzen du, ez `Id`. Dokumentazioan hori adierazita geratu da.
