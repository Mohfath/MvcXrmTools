<div dir="rtl">

<!-- nav-top -->
[« صفحه‌ی اصلی](../../README.md) › [اعداد](../../README.md#numeric)

# تبدیل عدد به حروف انگلیسی در Dynamics CRM (استپ Integer To Words)

این استپ یک عدد صحیح را به **حروف انگلیسی** تبدیل می‌کند؛ مثلاً `123` را به `one hundred and twenty-three`. برای نوشتن عدد به حروف **فارسی** از استپ **Convert Number To Persian String** (گروه Persian) استفاده کنید.

برای استفاده از این استپ، در طراحی Workflow از منوی **Add Step** استپ **Integer To Words** را انتخاب کنید.

## پارامترهای ورودی

* **Number (اجباری)** : عدد صحیح. بازه‌ی مجاز، بازه‌ی عدد صحیح ۳۲ بیتی است: از `-2,147,483,647` تا `2,147,483,647`.

## پارامتر خروجی

* **Number In Words** : عدد به حروف انگلیسی.

## مثال

| Number | Number In Words |
|---|---|
| `0` | `zero` |
| `7` | `seven` |
| `21` | `twenty-one` |
| `101` | `one hundred and one` |
| `123` | `one hundred and twenty-three` |
| `12345` | `twelve thousand three hundred and forty-five` |
| `-45` | `minus forty-five` |

## نکته‌ها

* عدد منفی با `minus` شروع می‌شود.
* رقم‌های دو تا نه‌تایی با خط تیره نوشته می‌شوند (`twenty-one`) و `and` پیش از باقی‌مانده‌ی کمتر از صد می‌آید (`one hundred and one`).
* اگر عدد دقیقاً مضربی از صد، هزار یا میلیون باشد (مثل `100` یا `1000`)، ممکن است در انتهای نتیجه یک فاصله (space) اضافه باشد؛ اگر نتیجه را با متن دیگری مقایسه می‌کنید، از [Trim](../string/trim.md) استفاده کنید.
* این استپ فقط عدد **صحیح** را قبول می‌کند؛ اعشار پشتیبانی نمی‌شود.

---

منبع: این استپ از مجموعه‌ی متن‌باز LAT.WorkflowUtilities (بخش Numeric) به این پروژه آورده شده است.

---

<!-- nav-bottom -->
[⬆ بازگشت به فهرست اصلی و همه‌ی استپ‌ها](../../README.md#steps)

</div>
