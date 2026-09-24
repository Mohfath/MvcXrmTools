<div dir="rtl">

<!-- nav-top -->
[« صفحه‌ی اصلی](../../README.md) › [امنیت و دسترسی](../../README.md#security)

# بررسی عضویت کاربر در تیم در Dynamics CRM (استپ Is User Member Of Team)

این استپ بررسی می‌کند که یک کاربر عضو یک تیم (Team) مشخص هست یا نه.

برای استفاده از این استپ، در طراحی Workflow از منوی **Add Step** به گروه **MvcTeam Security** بروید و **Is User Member Of Team** را انتخاب کنید.

## پارامترهای ورودی

* **User (اجباری)** : کاربر موردنظر (Lookup به کاربر).
* **Team (اجباری)** : تیم موردنظر (Lookup به تیم).

## پارامتر خروجی

* **Is Member** : اگر کاربر عضو تیم باشد `True` و در غیر این صورت `False` است.

## نکته‌ها

* فقط عضویت مستقیم در همان تیم بررسی می‌شود.
* برای بررسی نقش‌ها از [Is User Has Role](is-user-has-role.md) و [Is User Teams Have Role](is-user-teams-have-role.md) استفاده کنید.
* برای گرفتن کاربری که Workflow را شروع کرده از [Get Initiating User](get-initiating-user.md) استفاده کنید.

## مثال کاربرد

پیش از تخصیص یک رکورد به یک کاربر، بررسی کنید او عضو «تیم فروش» است؛ اگر نبود، رکورد را به مدیر بفرستید.

---

<!-- nav-bottom -->
[⬆ بازگشت به فهرست اصلی و همه‌ی استپ‌ها](../../README.md#steps)

</div>
