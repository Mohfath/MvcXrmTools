<div dir="rtl">

<!-- nav-top -->
[« صفحه‌ی اصلی](../../README.md) › [متن](../../README.md#string)

# کدگذاری متن به Base64 در Dynamics CRM (استپ B64 Encode)

این استپ یک متن را به Base64 تبدیل می‌کند. متن ابتدا به بایت‌های UTF-8 تبدیل و بعد کدگذاری می‌شود، پس متن فارسی هم درست کار می‌کند.

برای استفاده از این استپ، در طراحی Workflow از منوی **Add Step** استپ **B64 Encode** را انتخاب کنید.

## پارامترهای ورودی

* **String To Encode (اجباری)** : متنی که باید کدگذاری شود.

## پارامتر خروجی

* **B64 Encoded String** : متن کدگذاری‌شده به Base64.

## مثال

| String To Encode | B64 Encoded String |
|---|---|
| `Hello` | `SGVsbG8=` |
| `سلام` | `2LPZhNin2YU=` |

## نکته‌ها

* عکس این استپ [B64 Decode](b64-decode.md) است.

---

<!-- nav-bottom -->
[⬆ بازگشت به فهرست اصلی و همه‌ی استپ‌ها](../../README.md#steps)

</div>
