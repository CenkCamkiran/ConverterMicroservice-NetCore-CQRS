# K8S - Microservice App

- Singleton yapısında connection'lar kur! (OK!)

## Web Service Yapılacaklar

- Web service'de Farklı index'lere log atmayı öğren. (param olarak indexName) (OK!)
- Web service'de elk logging hata vermesi durumda Log4Net ile dosyaya log yaz. (OK!)
- Sonra bu log dosyalarını k8s'de ve dockerfile'da??? volume olarak tanımla.
- Web service'de file logging için elk da yaptığın gibi info log'ları ekle (OK!)
- Request ve response date'leri almaya çalış. (ARAŞTIR!)
- ELK logging'deki 2 ayrı fonksiyonu teke indirgemeye çalış. (OK!)

## Consumer Yapılacaklar

- Farklı index'lere log atmayı öğren. (param olarak indexName) (OK!)
- Web service'de elk logging hata vermesi durumda Log4Net ile dosyaya log yaz. (OK!)
- Sonra bu log dosyalarını k8s'de ve dockerfile'da??? volume olarak tanımla.
- Batch mi ya da farklı bişey olarak mı çalışacak? Karar ver. (ARAŞTIR!)
- MP4'ten Mp3'e convert eden library bul! (KODU YAZ!)
- Web service'deki gibi Singleton yapısında connection'lar kur! (ARAŞTIR!)
- Web service'de file logging için elk da yaptığın gibi info log'ları ekle (OK!)
- Temp olarak açılan dosyaları en son sil (ARAŞTIR!)

# İş tanımı

- Queue'dan mesajı al, mp3 çevir ve stream datasını elde et, stream datasını (mp3 dosyası, guid verisi) minio'ya yolla, başka bir queue'ya (mesela adı notif olsun) dosya guid'i ve email adresini yolla. converter kuyruğuna ack+ yolla!

## Notif Yapılacaklar

- Queue'ya gelen dosya guid'i ve email adresini al, mail olarak yolla! ARAŞTIR!)

## Logging Yapılacaklar

- Logging microservice'i draw.io'da çiz! (ARAŞTIR!)
- otherlogs ve errorlogs kuyrukların attığın model tiplerine karar ver. modeller farklı olursa ne olacak (Logging service'deki converter ve objStorage modelleri farklı, neye göre deserialize edilecek) (OK!)
- Logging microservice'de kuyrukları consume etmeyi async ya da thread ile paralel hallet! (ARAŞTIR!)
- Kuyruktan gelen modellere göre ELK'da index'leri oluştur. (ARAŞTIR!)

## K8S

- YAML paylaşılacaklar
- RAbbitMQ
- Minio
- ELK
- Kong (API Gateway) => Webservice dışarı açık olmayacak internal olacak, Kong'un admin url'i hariç diğer url dışarı açılacak (Basic auth ile)
- WebService
- Consumer
- NotifConsumer
- LoggingConsumer
