# Jatetxea API Dokumentazioa

Dokumentazio honek `GastuakApi` proiektuko controller-ak eta DTO-ak azaltzen ditu. Helburua da APIa erabiltzen duen pertsona batek jakitea zein ruta dauden, zertarako balio duten, zer datu bidali behar diren eta zer jasoko den erantzunean.

## Proiektuaren testuingurua

API honek jatetxe baten kudeaketa egiten du: inbentarioa, platerak, errezetak, mahaiak, erreserbak, langileak, zerbitzuak eta deskontuak. Backend-a ASP.NET Core 8 erabiliz eginda dago, NHibernate bidez datu-basearekin konektatzen da, eta Swagger/DocFX bidez dokumentatu da.

## Dokumentazioa nola ireki

1. APIaren dokumentazioa DocFXrekin sortzeko:

   ```powershell
   docfx docfx.json
   ```

2. Sortutako web dokumentazioa ikusteko:

   ```powershell
   docfx docfx.json --serve
   ```

3. APIa garapenean exekutatzen denean, Swagger UI helbide honetan dago:

   ```text
   /swagger
   ```

Dokumentazio osoa hurrengo orrian dago: [API dokumentazioa](api-dokumentazioa.md).
