﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* Ismn.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace ManagedIrbis.Identifiers
{
    //
    // Международный стандартный музыкальный номер
    // или ISMN (англ. International Standard Music Number)
    // — стандарт десятизначной буквенно-цифровой идентификации
    // нотных изданий. Стандарт разработан в ISO (ISO 10957).
    //
    // Международная стандартная нумерация музыкальных изданий
    // осуществляется под руководством Международного агентства
    // ISMN(англ. International ISMN Agency). Международное
    // агентство ISMN централизованно предоставляет номера ISMN
    // национальным агентствам ISMN стран-участ­ни­ков системы ISMN.
    // В Российской Федерации функции национального агентства
    // ISMN в 2007 году возложены на Российскую книжную палату.
    //
    // ISMN составляется из четырёх блоков, разделённых дефисами:
    //
    // префикс M (необходим в целях отличия от ISBN),
    // идентификатор издателя,
    // идентификатор конкретного издания,
    // одна контрольная цифра.
    //
    // Нотному изданию могут быть присвоены оба кода, как ISMN,
    // так и ISBN. В отличие от ISBN, в стандарт ISMN не заложено
    // разделение издателей по странам.
    //
    // Расчёт контрольной цифры производится по следующему алгоритму.
    // Каждый знак в коде ISMN умножается на его вес.
    // Веса знака — чередующиеся 3 и 1. Префикс M всегда имеет вес 3,
    // следующий знак — вес 1, следующий — вес 3, и так далее.
    // Префиксу М присвоено цифровое значение 3. Цифровые значения
    // умножаются на соответствующие веса знаков, затем полученные
    // значения складываются. Полученная сумма делится на 10 и от
    // получившегося частного берётся остаток. Он и будет
    // являться контрольной цифрой.
    //
    // Например, для ISMN-кода M-060-11561 расчёт контрольной цифры
    // будет таким:
    // 3xM + 1x0 + 3x6 + 1x0 + 3x1 + 1x1 + 3x5 + 1x6 + 3x1 =
    //  9  +  0  +  18 +  0  +  3  +  1  +  15 +  6  +  3  =  55
    //
    // Поскольку 55 mod 10=5 , контрольная цифра — 5, а полный
    // ISMN-код для данного издания: M-060-11561-5.
    //
    // См. https://ru.wikipedia.org/wiki/%D0%9C%D0%B5%D0%B6%D0%B4%D1%83%D0%BD%D0%B0%D1%80%D0%BE%D0%B4%D0%BD%D1%8B%D0%B9_%D1%81%D1%82%D0%B0%D0%BD%D0%B4%D0%B0%D1%80%D1%82%D0%BD%D1%8B%D0%B9_%D0%BC%D1%83%D0%B7%D1%8B%D0%BA%D0%B0%D0%BB%D1%8C%D0%BD%D1%8B%D0%B9_%D0%BD%D0%BE%D0%BC%D0%B5%D1%80
    // https://en.wikipedia.org/wiki/International_Standard_Music_Number



    class Ismn
    {
    }
}
