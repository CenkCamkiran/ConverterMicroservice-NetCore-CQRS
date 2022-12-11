#### K8S - Microservice App

Description....

### Web Service Yapılacaklar

Web service'de Farklı index'lere log atmayı öğren. (param olarak indexName)
Web service'de elk logging hata vermesi durumda Log4Net ile dosyaya log yaz. Sonra bu dosyayı k8s'de ve dockerfile'da??? volume olarak tanımla.

### Consumer Yapılacaklar

Farklı index'lere log atmayı öğren. (param olarak indexName)
elk logging hata vermesi durumda Log4Net ile dosyaya log yaz. Sonra bu dosyayı k8s'de ve dockerfile'da??? volume olarak tanımla.
Batch mi ya da farklı bişey olarak mı çalışacak? Karar ver.
MP4'ten Mp3'e convert eden library bul!
Web service'deki gibi Singleton yapısında connection'lar kur!

Queue'dan mesajı al, mp3 çevir ve stream datasını elde et, stream datasını (mp3 dosyası, guid verisi) minio'ya yolla, başka bir queue'ya (mesela adı notif olsun) dosya guid'i ve email adresini yolla. converter kuyruğuna ack+ yolla!

### Notif Yapılacaklar

Queue'ya gelen dosya guid'i ve email adresini al, mail olarak yolla!

### K8S

YAML paylaşılacaklar
RAbbitMQ
Minio
ELK
Kong (API Gateway) => Webservice dışarı açık olmayacak internal olacak, Kong'un admin url'i hariç diğer url dışarı açılacak (Basic auth ile)
WebService
Consumer
NotifConsumer
