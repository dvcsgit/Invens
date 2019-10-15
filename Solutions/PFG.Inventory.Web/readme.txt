
====What's this=====
必成成品倉儲管理系統
使用VS2013開啟

====主機環境=====
Server2008 R2
SQL Server 2012

====Web環境=====
ASP.NET MVC 5
.NET 4.5
Entity Framework 6.1.1 (NOT Code First)
ace 131 (版型)


====相關元件======
PagedList => 分頁元件 
http://kevintsengtw.blogspot.tw/2013/11/aspnet-mvc-pagedlistmvcajax.html#.VE7hvfmUfWa
https://github.com/troygoode/PagedList

Autofac 3.4.0 => DI 元件




====注意事項與參考=======
不再使用view CoreJS ,改用 Scripts/commonjs.js

UI HTML命名方式

== form ==
#查詢表單 query-form
#條件表單 criteria-form
#儲存表單 save-form

== button (單一 掛id ) ==
#新增按鈕 btn-create
#修改按鈕 btn-edit
#重設按鈕 btn-reset
#取消按鈕 btn-cancel
#匯出按鈕 btn-export

== grid action btn (清單 掛class )==

#編輯按鈕 btn-edit
#檢視按鈕 btn-detail
#刪除按鈕 btn-delete
#儲存按鈕 btn-save
#關閉按鈕 btn-close



Pattern
使用以下架構

http://weblogs.asp.net/shijuvarghese/releasing-socialgoal-reference-web-app-for-asp-net-mvc-5-ef-6-code-first-automapper-autofac-tdd-and-ddd
https://github.com/MarlabsInc/SocialGoal

====DB Migration 流程=====