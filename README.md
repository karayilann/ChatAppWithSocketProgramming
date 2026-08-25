# ChatAppWithSocketProgramming

`ChatAppWithSocketProgramming`, **C# ve Socket Programming** kullanılarak geliştirilmiş istemci/sunucu (client/server) tabanlı bir sohbet uygulamasıdır. Proje, TCP bağlantısı üzerinden metin mesajı alışverişi ve dosya transferi yapar.

## Proje Ne Yapar?

- Sunucu uygulaması (`ServerForm`) belirli bir IP/port üzerinde dinleme başlatır.
- İstemci uygulaması (`ClientForm`) sunucuya TCP ile bağlanır.
- Sunucu ve istemci arasında `NetworkStream` üzerinden metin mesajları gönderilir/alınır.
- Sunucu, bağlı istemcilere dosya gönderebilir.
- İstemci, gelen dosya meta bilgisini okuyup dosyayı seçilen klasöre kaydeder.

## Teknoloji ve Mimari

- **Dil:** C#
- **Framework:** .NET 8
- **UI:** Windows Forms
- **Ağ Katmanı:** `TcpListener`, `TcpClient`, `NetworkStream`
- **İletişim Modeli:** TCP tabanlı client/server

## Proje Yapısı

```text
/ChatAppWithSocketProgramming
├─ /ChatApplication                 # Sunucu UI projesi (Windows Forms)
│  ├─ Program.cs                    # Sunucuyu açan giriş noktası
│  ├─ ServerForm.cs                 # Sunucu ekranı, dinleme, mesaj/dosya gönderimi
│  └─ ChatApplication.sln           # Solution dosyası
├─ /ClientForm                      # İstemci UI projesi (Windows Forms)
│  ├─ Program.cs                    # İstemciyi açan giriş noktası
│  └─ ClientForm.cs                 # Sunucuya bağlanma, mesaj alma/gönderme, dosya alma
├─ /BusinessLogic
│  ├─ ServerService.cs              # Sunucu başlatma, client kabul etme, stream yönetimi
│  ├─ ClientService.cs              # İstemci bağlantısı başlatma/durdurma
│  └─ /UserLogin/ClientInfo.cs      # Bağlı istemciye ait temel bilgi (IP)
└─ /ChatApplication.Domain
   ├─ Message.cs                    # Mesaj modeli
   ├─ MessageService.cs             # Mesaj gönderme/alma işlemleri
   └─ FileTransferService.cs        # Dosya gönderme/alma işlemleri
```

## Uygulamalar Nasıl Başlar?

### Sunucu
- Giriş noktası: `ChatApplication/Program.cs`
- Çalıştırıldığında `Application.Run(new ServerForm())` ile sunucu arayüzü açılır.
- `ServerForm` üzerinde IP ve port girilip **Listen** ile `ServerService.StartServer(ip, port)` çağrılır.

### İstemci
- Giriş noktası: `ClientForm/Program.cs`
- Çalıştırıldığında `Application.Run(new ClientForm())` ile istemci arayüzü açılır.
- IP ve port girilip **Connect** ile `ClientService.StartClient(ip, port)` çağrılır.

## Sınıflar ve Servisler

### `ServerService`
`BusinessLogic/ServerService.cs`

- `StartServer(ip, port)`: `TcpListener` ile sunucuyu başlatır.
- `AcceptClient()`: Yeni istemciyi kabul eder, `NetworkStream` alır ve listelerde tutar.
- `ClientStreams`: Bağlı istemcilerin stream listesini dışarı açar.
- `BroadcastMessage(...)`: Tüm istemcilere veri yazma (şu an aktif akışta kullanılmıyor).
- `StopServer()`: İstemcileri, stream’leri ve sunucuyu kapatır.

### `ClientService`
`BusinessLogic/ClientService.cs`

- `StartClient(ip, port)`: `TcpClient` ile sunucuya bağlanır, `NetworkStream` alır.
- `StopClient()`: İstemci bağlantısını ve stream’i kapatır.
- `NetworkStream`/`Client` property’leri ile bağlantı nesnelerine erişim sağlar.

### `MessageService`
`ChatApplication.Domain/MessageService.cs`

- `SendMessage(Message message)`: Mesajı string’e çevirip ASCII byte dizisi olarak `NetworkStream`’e yazar.
- `ReceiveMessage()`: Stream’den veri okuyup `Message` nesnesi üretir.
- `Read(...)` sonucu `0` ise bağlantının kapandığını kabul edip `null` döndürür.

### `Message`
`ChatApplication.Domain/Message.cs`

- `Content` ve `Timestamp` alanlarını taşır.
- Metin mesajları için `Message(string content)` kurucusu kullanılır.
- Dosya bildirimi için `Message(string fileName, long fileSize)` kurucusu `FILE: ...|SIZE: ...` formatlı içerik üretir.
- `ToString()` sadece `Content` döndürür.

### `FileTransferService`
`ChatApplication.Domain/FileTransferService.cs`

- `SendFileToAllAsync(...)`: Dosyayı tüm istemci stream’lerine gönderir.
- `SendFileToClientAsync(...)`: Dosyayı tek istemciye gönderir.
- `ReceiveFileAsync(...)`: Beklenen byte sayısı tamamlanana kadar okuyup diske yazar.
- `ValidateFileSize(...)`: İndirilen dosya boyutu doğrulaması yapar.

### `ClientInfo`
`BusinessLogic/UserLogin/ClientInfo.cs`

- Şu an bağlı istemci için temel olarak IP bilgisini taşır.
- Sunucu tarafında istemci listesini ekranda göstermek için kullanılır.

## `NetworkStream` Üzerinden Mesaj Akışı

Mesajlaşma temel olarak ASCII byte akışıyla yapılır:

```csharp
// Gönderim (özet)
string content = message.ToString();
byte[] data = Encoding.ASCII.GetBytes(content);
_networkStream.Write(data, 0, data.Length);

// Alım (özet)
byte[] data = new byte[1024];
int receivedBytes = _networkStream.Read(data, 0, data.Length);
string content = Encoding.ASCII.GetString(data, 0, receivedBytes);
```

Akış şu şekildedir:
1. Gönderen taraf `MessageService.SendMessage` ile yazma yapar.
2. Alıcı taraf `MessageService.ReceiveMessage` ile okur.
3. Sunucu, bağlı istemciler için ayrı alım thread’leriyle sürekli dinleme yapar.
4. İstemci, bağlantı açık kaldığı sürece döngü içinde gelen verileri işler.

## Dosya Gönderme/Alma Akışı

Dosya gönderiminde önce bir meta veri başlığı iletilir, ardından dosya byte’ları gönderilir.

### Meta veri formatı
Kodda kullanılan format:

```text
FILE:{name}|SIZE:{size}|
```

Örnek:

```text
FILE:ornek.pdf|SIZE:24576|
```

### Süreç
1. Sunucu `FileTransferService` ile dosya adı ve boyutunu içeren meta veriyi stream’e yazar.
2. Hemen ardından dosya içeriğini parça parça (`1024` byte buffer) gönderir.
3. İstemci gelen mesaj `FILE:` ile başlıyorsa meta veriyi parse eder.
4. İstemci `ReceiveFileAsync` ile belirtilen byte sayısı tamamlanana kadar okuyup dosyayı kaydeder.

## Kurulum ve Çalıştırma

> Not: Proje Windows Forms olduğu için Windows ortamında Visual Studio veya .NET SDK ile çalıştırılması önerilir.

### Gereksinimler

- .NET 8 SDK
- Windows (WinForms çalıştırmak için)

### Kaynak kodu hazırlama

```bash
git clone https://github.com/karayilann/ChatAppWithSocketProgramming.git
cd ChatAppWithSocketProgramming
```

### Solution derleme

```bash
dotnet build /home/runner/work/ChatAppWithSocketProgramming/ChatAppWithSocketProgramming/ChatApplication/ChatApplication.sln
```

### Sunucu uygulamasını çalıştırma

```bash
dotnet run --project /home/runner/work/ChatAppWithSocketProgramming/ChatAppWithSocketProgramming/ChatApplication/ChatApplication.ServerUI.csproj
```

### İstemci uygulamasını çalıştırma

```bash
dotnet run --project /home/runner/work/ChatAppWithSocketProgramming/ChatAppWithSocketProgramming/ClientForm/ChatApplication.ClientUI.csproj
```

## Kullanım

1. Sunucuyu açın, IP/port girip dinlemeyi başlatın.
2. İstemciyi açın, aynı IP/port ile bağlanın.
3. İstemciden veya sunucudan metin mesajı gönderin.
4. Dosya alım yeri için istemcide klasör seçin.
5. Sunucudan dosya gönderin; istemci dosyayı seçilen klasöre kaydeder.

## Geliştirme Notları

- Mesajlar ASCII ile taşınıyor; UTF-8’e geçiş Türkçe karakter desteğini iyileştirir.
- Mesaj ve dosya protokolü şu an düz metin/parsing yaklaşımı kullanıyor; daha sağlam bir çerçeveleme protokolü eklenebilir.
- `ClientInfo` sınıfı kullanıcı adı/e-posta gibi alanlarla genişletilebilir.
- Hata yönetimi ve bağlantı kopma senaryoları daha detaylı ele alınabilir.
- Asenkron ve iptal edilebilir (`CancellationToken`) iletişim yapısı eklenebilir.
- Loglama ve test kapsamı artırılarak bakım kolaylaştırılabilir.
