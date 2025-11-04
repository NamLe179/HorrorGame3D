# HorrorGame3D

* Cài đặt Unity Hub
* Cài đặt Unity Version 2022.3.62f1 (là phiên bản LTS)
* Vào Project chọn Add project from disk, chọn thư mục chứa project
* Mở Project đợi Unity tải thư viện,...
* Khi làm các object hoặc bất cứ thứ gì thì tạo 1 nhánh ra rồi làm, push, commit,...
* Nếu chưa add asset, vào link sau, tải và giải nén 'Game Assets.rar' ở folder Assets. Link drive: https://drive.google.com/drive/folders/1tBLeJh2V4lo5TnQGFnA-JijbKYOQ6X9j?usp=drive\_link
* Nếu đã cài sẵn asset và không muốn tải lại asset, tải và import thêm đèn pin, camera (51, 52) cùng file audio 47 như trong sheet: https://docs.google.com/spreadsheets/d/1D9Fu3UpdtRA7giubjWM9B954SlLPdYC0nxeVUhMqlus/edit?usp=sharing. Tuy nhiên, asset mới nhất đã sắp xếp lại thứ tự thư mục 1 chút để dễ xác định asset cần sử dụng.



Lưu Ý Về Các File C# Thư Mục Assets/Scripts Chỉnh Sửa Hệ Thống Tương Tác Vật Thể:

* Nhớ Thêm Asset SocketsAndSwitches và thay thế Apartment Asset cũ trong Game Asset bằng Apartment Asset mới( Cả 2 thư mục zip đều trong Apartment Asset( Sau khi sửa code tương tác) trên Driver).
* Interactable.cs: Interface
* InteractableBase.cs: Abstract Class
* InteractableLightBase.cs: Abstract Class cho các đèn
* KeyData.cs: Component cho key để nhận diện key yêu cầu của vật thể
* LampInteract.cs: Component cho các đèn
* PlayerInteraction.cs: Component cho Player để tương tác
* LightSwitchInteration.cs: Component cho công tắc đèn
