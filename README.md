# K8S - Microservice App

## Web Service Yapılacaklar

- Web service'de Farklı index'lere log atmayı öğren. (param olarak indexName) (OK!)
- Web service'de elk logging hata vermesi durumda Log4Net ile dosyaya log yaz. (OK!)
- Sonra bu log dosyalarını k8s'de ve dockerfile'da??? volume olarak tanımla. (ARAŞTIR!)
- Web service'de file logging için elk da yaptığın gibi info log'ları ekle (OK!)
- Request ve response date'leri almaya çalış. (OK!)
- ELK logging'deki 2 ayrı fonksiyonu teke indirgemeye çalış. (OK!)

## Consumer Yapılacaklar

- Farklı index'lere log atmayı öğren. (param olarak indexName) (OK!)
- Web service'de elk logging hata vermesi durumda Log4Net ile dosyaya log yaz. (OK!)
- Sonra bu log dosyalarını k8s'de ve dockerfile'da??? volume olarak tanımla. (ARAŞTIR!)
- Batch mi ya da farklı bişey olarak mı çalışacak? Karar ver. (OK!)
- MP4'ten Mp3'e convert eden library bul! (OK!)
- Web service'deki gibi Singleton yapısında connection'lar kur! (OK!)
- Web service'de file logging için elk da yaptığın gibi info log'ları ekle (OK!)
- Temp olarak açılan dosyaları en son sil (OK!)

# İş tanımı

- Queue'dan mesajı al, mp3 çevir ve stream datasını elde et, stream datasını (mp3 dosyası, guid verisi) minio'ya yolla, başka bir queue'ya (mesela adı notif olsun) dosya guid'i ve email adresini yolla. converter kuyruğuna ack+ yolla!

## Notif Yapılacaklar

- Queue'ya gelen dosya guid'i ve email adresini al, mail olarak yolla! (OK!)

## Logging Yapılacaklar

- Logging microservice'i draw.io'da çiz! (ARAŞTIR!)
- Logging microservice'de kuyrukları consume etmeyi async ya da thread ile paralel hallet! (ARAŞTIR!)
- Kuyruktan gelen modellere göre ELK'da index'leri oluştur. (ARAŞTIR!)
- otherlogs ve errorlogs kuyrukların attığın model tiplerine karar ver. modeller farklı olursa ne olacak (Logging service'deki converter ve objStorage modelleri farklı, neye göre deserialize edilecek) (OK!)

- otherlogs ve errorlogs kuyurkları için açılan elk indexlerini ingest pipeline ile reindex yap. yeni index'e taşı. eski index leri sil. (ARAŞTIR!)
- Mail server ve domain araştır. (ARAŞTIR!)
- Notif service'de mail server için env variables'ları düzenle. (ARAŞTIR!)
- Minio object ttl ekle! (ARAŞTIR!)
- rabbitmq message ttl ekle! (ARAŞTIR!)

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
