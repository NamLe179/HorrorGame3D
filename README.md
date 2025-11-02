# HorrorGame3D

* Cài đặt Unity Hub
* Cài đặt Unity Version 2022.3.62f1 (là phiên bản LTS)
* Vào Project chọn Add project from disk, chọn thư mục chứa project
* Mở Project đợi Unity tải thư viện,...
* Khi làm các object hoặc bất cứ thứ gì thì tạo 1 nhánh ra rồi làm, push, commit,...
* Nếu chưa add asset, vào link sau, tải và giải nén 'Game Assets.rar' ở folder Assets. Link drive: https://drive.google.com/drive/folders/1tBLeJh2V4lo5TnQGFnA-JijbKYOQ6X9j?usp=drive\_link
* Nếu đã cài sẵn asset và không muốn tải lại asset, tải và import thêm đèn pin, camera (51, 52) cùng file audio 47 như trong sheet: https://docs.google.com/spreadsheets/d/1D9Fu3UpdtRA7giubjWM9B954SlLPdYC0nxeVUhMqlus/edit?usp=sharing. Tuy nhiên, asset mới nhất đã sắp xếp lại thứ tự thư mục 1 chút để dễ xác định asset cần sử dụng.



Lưu Ý Về Các File C# Thư Mục Assets/Scripts Chỉnh Sửa Hệ Thống Tương Tác Vật Thể:

* Interactable.cs: Interface
* InteractableBase.cs: Abstract Class
* InteractableBase.cs: Abstract Class cho các đèn
* KeyData.cs: Component cho key để nhận diện key yêu cầu của vật thể
* LampInteract.cs: Component cho các đèn
* PlayerInteraction.cs: Component cho Player để tương tác
* Thư mục \_BPS Basic Assets, Apartment Asset: Folder chứa các file C# tương ứng trong Assets\\Game Assets\\Apartment Asset\\Apartment Asset/\_BPS Basic Assets, và Assets\\Game Assets\\Apartment Asset\\Apartment Asset\\Apartment Kit. Trong đó các file C# được để trong folder cùng tên y như model của căn nhà. Bên cạnh đó các file đã được đổi tên vì vậy cần chú ý đọc tên ( để ý chỉ số phụ 1, 2, 3) để coppy paste code đúng file.



