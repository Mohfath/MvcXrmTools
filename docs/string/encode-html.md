<div dir="rtl">

<!-- nav-top -->
[« صفحه‌ی اصلی](../../README.md) › [متن](../../README.md#string)

# کدگذاری نویسه‌های HTML در Dynamics CRM (استپ Encode Html)

این استپ نویسه‌های خاص HTML را در یک متن به معادل امن آن‌ها تبدیل می‌کند (`<` ← `&lt;`، `>` ← `&gt;`، `&` ← `&amp;` و مانند آن). برای اینکه متنی که شامل این نویسه‌هاست، در ایمیل یا صفحه‌ی HTML به‌عنوان تگ تفسیر نشود.

برای استفاده از این استپ، در طراحی Workflow از منوی **Add Step** استپ **Encode Html** را انتخاب کنید.

## پارامترهای ورودی

* **String To Encode (اجباری)** : متنی که باید کدگذاری شود.

## پارامتر خروجی

* **Encoded String** : متن کدگذاری‌شده.

## مثال

| String To Encode | Encoded String |
|---|---|
| `<b>Tom & Jerry</b>` | `&lt;b&gt;Tom &amp; Jerry&lt;/b&gt;` |

## نکته‌ها

* عکس این استپ [Decode Html](decode-html.md) است.
* برای کدگذاری بخشی از آدرس اینترنتی از [Url Encode](url-encode.md) استفاده کنید.

---

<!-- nav-bottom -->
[⬆ بازگشت به فهرست اصلی و همه‌ی استپ‌ها](../../README.md#steps)

</div>
