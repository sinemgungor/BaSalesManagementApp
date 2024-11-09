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

   ![Ekran görüntüsü 2024-11-09 163617](https://github.com/user-attachments/assets/d866b269-7ba0-40d0-a92e-3db87549b045)
![Ekran görüntüsü 2024-11-09 163124](https://github.com/user-attachments/assets/f8216aff-e91c-4995-9c1b-92801293a27a)

![Ekran görüntüsü 2024-11-09 163133](https://github.com/user-attachments/assets/7a7d6e69-56fa-4302-8146-7b649167b81a)

![Ekran görüntüsü 2024-11-09 163143](https://github.com/user-attachments/assets/68e27c4f-a8ab-4998-baa3-9bf27cd3aed5)

![Ekran görüntüsü 2024-11-09 163156](https://github.com/user-attachments/assets/27a6a8ab-891e-4d26-9d03-ba22fae33fc5)

![Ekran görüntüsü 2024-11-09 163208](https://github.com/user-attachments/assets/56e54ce4-7feb-4409-ae2b-f369093c5201)

![Ekran görüntüsü 2024-11-09 163343](https://github.com/user-attachments/assets/46919698-582a-4b35-a154-d3be0702ddf1)

![Ekran görüntüsü 2024-11-09 163352](https://github.com/user-attachments/assets/b9729a4a-aea5-4326-bf23-01e4881bd009)

![Ekran görüntüsü 2024-11-09 163404](https://github.com/user-attachments/assets/22c57194-f5f7-4ed6-8a32-db9c813ff4c3)

![Ekran görüntüsü 2024-11-09 163415](https://github.com/user-attachments/assets/28b776cf-503c-41ed-b2ad-985982381f94)

![Ekran görüntüsü 2024-11-09 163423](https://github.com/user-attachments/assets/93cd2a0a-821b-433e-a815-e80771a3e490)

![Ekran görüntüsü 2024-11-09 163429](https://github.com/user-attachments/assets/ea5c5b0a-8b8f-417e-961a-0b07737cad48)

![Ekran görüntüsü 2024-11-09 163436](https://github.com/user-attachments/assets/7db42a5c-fdc0-478e-8a0f-6c3150d65796)

![Ekran görüntüsü 2024-11-09 163445](https://github.com/user-attachments/assets/e2d1da22-8b4b-4e77-8129-4ade90fa065c)

![Ekran görüntüsü 2024-11-09 163455](https://github.com/user-attachments/assets/ee119451-cb27-4607-87fe-1469c494f353)

![Ekran görüntüsü 2024-11-09 163502](https://github.com/user-attachments/assets/f7eeac20-7878-4c82-a9d8-85dcfbfb7906)

![Ekran görüntüsü 2024-11-09 163511](https://github.com/user-attachments/assets/0915ed4e-e8f9-4299-9521-046cfb37c679)

![Ekran görüntüsü 2024-11-09 163525](https://github.com/user-attachments/assets/bc5d8e1a-103c-40cd-8179-9fe0f79c576f)

![Ekran görüntüsü 2024-11-09 163531](https://github.com/user-attachments/assets/b0d0a097-59a6-4186-b29f-d0957e1450fa)

![Ekran görüntüsü 2024-11-09 163545](https://github.com/user-attachments/assets/ab2c3873-b67a-489d-8dfa-7ccb8581a28c)

![Ekran görüntüsü 2024-11-09 163611](https://github.com/user-attachments/assets/bbdea7d2-2d5a-444d-be90-956b5400f03c)
