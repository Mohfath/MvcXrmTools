<div dir="rtl">

<!-- nav-top -->
[« صفحه‌ی اصلی](../../README.md) › [متن](../../README.md#string)

# رمزگشایی Base64 در Dynamics CRM (استپ B64 Decode)

این استپ یک متن Base64 را به متن اصلی برمی‌گرداند. بایت‌ها به‌صورت UTF-8 خوانده می‌شوند، پس متن فارسی هم درست برمی‌گردد.

برای استفاده از این استپ، در طراحی Workflow از منوی **Add Step** استپ **B64 Decode** را انتخاب کنید.

## پارامترهای ورودی

* **String To Decode (اجباری)** : متن Base64.

## پارامتر خروجی

* **B64 Decoded String** : متن اصلی.

## مثال

| String To Decode | B64 Decoded String |
|---|---|
| `SGVsbG8=` | `Hello` |
| `2LPZhNin2YU=` | `سلام` |

## نکته‌ها

* اگر متن ورودی Base64 معتبر نباشد (مثلاً `abc`)، استپ با خطای `Invalid length for a Base-64 char array or string.` متوقف می‌شود.
* عکس این استپ [B64 Encode](b64-encode.md) است.

---

<!-- nav-bottom -->
[⬆ بازگشت به فهرست اصلی و همه‌ی استپ‌ها](../../README.md#steps)

</div>
