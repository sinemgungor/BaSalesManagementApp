# BaSalesManagementApp

BaSalesManagementApp, işletmelerin satış yönetimini kolaylaştırmak ve verimliliği artırmak için geliştirilmiş bir uygulamadır. Bu uygulama, kullanıcı dostu bir arayüzle işletme yöneticilerine ve çalışanlarına kapsamlı bir satış yönetimi deneyimi sunar.

## Özellikler

- Satış yönetimi ve izleme
- Müşteri ilişkileri yönetimi (CRM) entegrasyonu
- Ürün ve envanter takibi
- Raporlama ve analiz araçları

## Kullanılan Teknolojiler

- **ASP.NET Core MVC**: Uygulamanın temel yapısı ve MVC (Model-View-Controller) mimarisi için kullanıldı.
- **Entity Framework Core**: Veritabanı işlemleri için Code First yaklaşımı kullanılarak ORM (Object-Relational Mapping) aracı olarak tercih edildi.
- **Microsoft SQL Server**: Veritabanı olarak kullanıldı.
- **Identity Framework**: Kullanıcı kimlik doğrulaması ve yetkilendirme işlemleri için kullanıldı.
- **AutoMapper**: Veriler arasında kolay ve hızlı dönüşüm sağlamak için kullanıldı.
- **JavaScript ve jQuery**: Dinamik işlemler ve istemci tarafı doğrulama işlemleri için.
- **CSS (Bootstrap)**: Kullanıcı arayüzünün oluşturulması ve mobil uyumluluk için Bootstrap kullanıldı.

## Proje Yapısı

1. **Controllers**: İş mantığını barındıran ve kullanıcının isteğini işleyen sınıflar.
2. **Models**: Veritabanı yapısını temsil eden ve uygulamanın verileriyle ilgili işlemleri yapan sınıflar.
3. **Views**: Kullanıcıya gösterilen ve kullanıcıyla etkileşime girilen arayüzler.
4. **Data**: Veritabanı bağlantılarını ve `DbContext` sınıfını barındırır.
5. **Services**: Uygulamanın çekirdek servislerini sağlayan katman (isteğe bağlı olarak eklendi).

## Kurulum

1. **Projeyi Klonlayın**:
   ```bash
   git clone https://github.com/kullaniciadi/BaSalesManagementApp.git
