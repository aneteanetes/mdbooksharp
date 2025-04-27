"use strict";
/*!
 * Snowball JavaScript Library v0.3
 * http://code.google.com/p/urim/
 * http://snowball.tartarus.org/
 *
 * Copyright 2010, Oleg Mazko
 * http://www.mozilla.org/MPL/
 */

/**
 * export the module via AMD, CommonJS or as a browser global
 * Export code from https://github.com/umdjs/umd/blob/master/returnExports.js
 */
window.addEventListener("keydown", function (e) {
    if (e.keyCode === 114 || (e.ctrlKey && e.keyCode === 70)) {
        e.preventDefault();
    }
})
;(function (root, factory) {
    if (typeof define === 'function' && define.amd) {
        // AMD. Register as an anonymous module.
        define(factory)
    } else if (typeof exports === 'object') {
        /**
         * Node. Does not work with strict CommonJS, but
         * only CommonJS-like environments that support module.exports,
         * like Node.
         */
        module.exports = factory()
    } else {
        // Browser globals (root is window)
        factory()(root.lunr);
    }
}(this, function () {
    /**
     * Just return a value to define the module export.
     * This example returns an object, but the module
     * can return a function as the exported value.
     */
    return function(lunr) {
        /* provides utilities for the included stemmers */
        lunr.stemmerSupport = {
            Among: function(s, substring_i, result, method) {
                this.toCharArray = function(s) {
                    var sLength = s.length, charArr = new Array(sLength);
                    for (var i = 0; i < sLength; i++)
                        charArr[i] = s.charCodeAt(i);
                    return charArr;
                };

                if ((!s && s != "") || (!substring_i && (substring_i != 0)) || !result)
                    throw ("Bad Among initialisation: s:" + s + ", substring_i: "
                        + substring_i + ", result: " + result);
                this.s_size = s.length;
                this.s = this.toCharArray(s);
                this.substring_i = substring_i;
                this.result = result;
                this.method = method;
            },
            SnowballProgram: function() {
                var current;
                return {
                    bra : 0,
                    ket : 0,
                    limit : 0,
                    cursor : 0,
                    limit_backward : 0,
                    setCurrent : function(word) {
                        current = word;
                        this.cursor = 0;
                        this.limit = word.length;
                        this.limit_backward = 0;
                        this.bra = this.cursor;
                        this.ket = this.limit;
                    },
                    getCurrent : function() {
                        var result = current;
                        current = null;
                        return result;
                    },
                    in_grouping : function(s, min, max) {
                        if (this.cursor < this.limit) {
                            var ch = current.charCodeAt(this.cursor);
                            if (ch <= max && ch >= min) {
                                ch -= min;
                                if (s[ch >> 3] & (0X1 << (ch & 0X7))) {
                                    this.cursor++;
                                    return true;
                                }
                            }
                        }
                        return false;
                    },
                    in_grouping_b : function(s, min, max) {
                        if (this.cursor > this.limit_backward) {
                            var ch = current.charCodeAt(this.cursor - 1);
                            if (ch <= max && ch >= min) {
                                ch -= min;
                                if (s[ch >> 3] & (0X1 << (ch & 0X7))) {
                                    this.cursor--;
                                    return true;
                                }
                            }
                        }
                        return false;
                    },
                    out_grouping : function(s, min, max) {
                        if (this.cursor < this.limit) {
                            var ch = current.charCodeAt(this.cursor);
                            if (ch > max || ch < min) {
                                this.cursor++;
                                return true;
                            }
                            ch -= min;
                            if (!(s[ch >> 3] & (0X1 << (ch & 0X7)))) {
                                this.cursor++;
                                return true;
                            }
                        }
                        return false;
                    },
                    out_grouping_b : function(s, min, max) {
                        if (this.cursor > this.limit_backward) {
                            var ch = current.charCodeAt(this.cursor - 1);
                            if (ch > max || ch < min) {
                                this.cursor--;
                                return true;
                            }
                            ch -= min;
                            if (!(s[ch >> 3] & (0X1 << (ch & 0X7)))) {
                                this.cursor--;
                                return true;
                            }
                        }
                        return false;
                    },
                    eq_s : function(s_size, s) {
                        if (this.limit - this.cursor < s_size)
                            return false;
                        for (var i = 0; i < s_size; i++)
                            if (current.charCodeAt(this.cursor + i) != s.charCodeAt(i))
                                return false;
                        this.cursor += s_size;
                        return true;
                    },
                    eq_s_b : function(s_size, s) {
                        if (this.cursor - this.limit_backward < s_size)
                            return false;
                        for (var i = 0; i < s_size; i++)
                            if (current.charCodeAt(this.cursor - s_size + i) != s
                                .charCodeAt(i))
                                return false;
                        this.cursor -= s_size;
                        return true;
                    },
                    find_among : function(v, v_size) {
                        var i = 0, j = v_size, c = this.cursor, l = this.limit, common_i = 0, common_j = 0, first_key_inspected = false;
                        while (true) {
                            var k = i + ((j - i) >> 1), diff = 0, common = common_i < common_j
                                ? common_i
                                : common_j, w = v[k];
                            for (var i2 = common; i2 < w.s_size; i2++) {
                                if (c + common == l) {
                                    diff = -1;
                                    break;
                                }
                                diff = current.charCodeAt(c + common) - w.s[i2];
                                if (diff)
                                    break;
                                common++;
                            }
                            if (diff < 0) {
                                j = k;
                                common_j = common;
                            } else {
                                i = k;
                                common_i = common;
                            }
                            if (j - i <= 1) {
                                if (i > 0 || j == i || first_key_inspected)
                                    break;
                                first_key_inspected = true;
                            }
                        }
                        while (true) {
                            var w = v[i];
                            if (common_i >= w.s_size) {
                                this.cursor = c + w.s_size;
                                if (!w.method)
                                    return w.result;
                                var res = w.method();
                                this.cursor = c + w.s_size;
                                if (res)
                                    return w.result;
                            }
                            i = w.substring_i;
                            if (i < 0)
                                return 0;
                        }
                    },
                    find_among_b : function(v, v_size) {
                        var i = 0, j = v_size, c = this.cursor, lb = this.limit_backward, common_i = 0, common_j = 0, first_key_inspected = false;
                        while (true) {
                            var k = i + ((j - i) >> 1), diff = 0, common = common_i < common_j
                                ? common_i
                                : common_j, w = v[k];
                            for (var i2 = w.s_size - 1 - common; i2 >= 0; i2--) {
                                if (c - common == lb) {
                                    diff = -1;
                                    break;
                                }
                                diff = current.charCodeAt(c - 1 - common) - w.s[i2];
                                if (diff)
                                    break;
                                common++;
                            }
                            if (diff < 0) {
                                j = k;
                                common_j = common;
                            } else {
                                i = k;
                                common_i = common;
                            }
                            if (j - i <= 1) {
                                if (i > 0 || j == i || first_key_inspected)
                                    break;
                                first_key_inspected = true;
                            }
                        }
                        while (true) {
                            var w = v[i];
                            if (common_i >= w.s_size) {
                                this.cursor = c - w.s_size;
                                if (!w.method)
                                    return w.result;
                                var res = w.method();
                                this.cursor = c - w.s_size;
                                if (res)
                                    return w.result;
                            }
                            i = w.substring_i;
                            if (i < 0)
                                return 0;
                        }
                    },
                    replace_s : function(c_bra, c_ket, s) {
                        var adjustment = s.length - (c_ket - c_bra), left = current
                            .substring(0, c_bra), right = current.substring(c_ket);
                        current = left + s + right;
                        this.limit += adjustment;
                        if (this.cursor >= c_ket)
                            this.cursor += adjustment;
                        else if (this.cursor > c_bra)
                            this.cursor = c_bra;
                        return adjustment;
                    },
                    slice_check : function() {
                        if (this.bra < 0 || this.bra > this.ket || this.ket > this.limit
                            || this.limit > current.length)
                            throw ("faulty slice operation");
                    },
                    slice_from : function(s) {
                        this.slice_check();
                        this.replace_s(this.bra, this.ket, s);
                    },
                    slice_del : function() {
                        this.slice_from("");
                    },
                    insert : function(c_bra, c_ket, s) {
                        var adjustment = this.replace_s(c_bra, c_ket, s);
                        if (c_bra <= this.bra)
                            this.bra += adjustment;
                        if (c_bra <= this.ket)
                            this.ket += adjustment;
                    },
                    slice_to : function() {
                        this.slice_check();
                        return current.substring(this.bra, this.ket);
                    },
                    eq_v_b : function(s) {
                        return this.eq_s_b(s.length, s);
                    }
                };
            }
        };

        lunr.trimmerSupport = {
            generateTrimmer: function(wordCharacters) {
                var startRegex = new RegExp("^[^" + wordCharacters + "]+")
                var endRegex = new RegExp("[^" + wordCharacters + "]+$")

                return function(token) {
                    return token
                        .replace(startRegex, '')
                        .replace(endRegex, '');
                };
            }
        }
    }
}));
/*!
 * Lunr languages, `Russian` language
 * https://github.com/MihaiValentin/lunr-languages
 *
 * Copyright 2014, Mihai Valentin
 * http://www.mozilla.org/MPL/
 */
/*!
 * based on
 * Snowball JavaScript Library v0.3
 * http://code.google.com/p/urim/
 * http://snowball.tartarus.org/
 *
 * Copyright 2010, Oleg Mazko
 * http://www.mozilla.org/MPL/
 */

/**
 * export the module via AMD, CommonJS or as a browser global
 * Export code from https://github.com/umdjs/umd/blob/master/returnExports.js
 */
;
(function(root, factory) {
  if (typeof define === 'function' && define.amd) {
    // AMD. Register as an anonymous module.
    define(factory)
  } else if (typeof exports === 'object') {
    /**
     * Node. Does not work with strict CommonJS, but
     * only CommonJS-like environments that support module.exports,
     * like Node.
     */
    module.exports = factory()
  } else {
    // Browser globals (root is window)
    factory()(root.lunr);
  }
}(this, function() {
  /**
   * Just return a value to define the module export.
   * This example returns an object, but the module
   * can return a function as the exported value.
   */
  return function(lunr) {
    /* throw error if lunr is not yet included */
    if ('undefined' === typeof lunr) {
      throw new Error('Lunr is not present. Please include / require Lunr before this script.');
    }

    /* throw error if lunr stemmer support is not yet included */
    if ('undefined' === typeof lunr.stemmerSupport) {
      throw new Error('Lunr stemmer support is not present. Please include / require Lunr stemmer support before this script.');
    }

    /* register specific locale function */
    lunr.ru = function() {
      this.pipeline.reset();
      this.pipeline.add(
        lunr.ru.trimmer,
        lunr.ru.stopWordFilter,
        lunr.ru.stemmer
      );
    };

    /* lunr trimmer function */
    lunr.ru.wordCharacters = "\u0400-\u0484\u0487-\u052F\u1D2B\u1D78\u2DE0-\u2DFF\uA640-\uA69F\uFE2E\uFE2F";
    lunr.ru.trimmer = lunr.trimmerSupport.generateTrimmer(lunr.ru.wordCharacters);

    lunr.Pipeline.registerFunction(lunr.ru.trimmer, 'trimmer-ru');

    /* lunr stemmer function */
    lunr.ru.stemmer = (function() {
      /* create the wrapped stemmer object */
      var Among = lunr.stemmerSupport.Among,
        SnowballProgram = lunr.stemmerSupport.SnowballProgram,
        st = new function RussianStemmer() {
          var a_0 = [new Among("\u0432", -1, 1), new Among("\u0438\u0432", 0, 2),
              new Among("\u044B\u0432", 0, 2),
              new Among("\u0432\u0448\u0438", -1, 1),
              new Among("\u0438\u0432\u0448\u0438", 3, 2),
              new Among("\u044B\u0432\u0448\u0438", 3, 2),
              new Among("\u0432\u0448\u0438\u0441\u044C", -1, 1),
              new Among("\u0438\u0432\u0448\u0438\u0441\u044C", 6, 2),
              new Among("\u044B\u0432\u0448\u0438\u0441\u044C", 6, 2)
            ],
            a_1 = [
              new Among("\u0435\u0435", -1, 1), new Among("\u0438\u0435", -1, 1),
              new Among("\u043E\u0435", -1, 1), new Among("\u044B\u0435", -1, 1),
              new Among("\u0438\u043C\u0438", -1, 1),
              new Among("\u044B\u043C\u0438", -1, 1),
              new Among("\u0435\u0439", -1, 1), new Among("\u0438\u0439", -1, 1),
              new Among("\u043E\u0439", -1, 1), new Among("\u044B\u0439", -1, 1),
              new Among("\u0435\u043C", -1, 1), new Among("\u0438\u043C", -1, 1),
              new Among("\u043E\u043C", -1, 1), new Among("\u044B\u043C", -1, 1),
              new Among("\u0435\u0433\u043E", -1, 1),
              new Among("\u043E\u0433\u043E", -1, 1),
              new Among("\u0435\u043C\u0443", -1, 1),
              new Among("\u043E\u043C\u0443", -1, 1),
              new Among("\u0438\u0445", -1, 1), new Among("\u044B\u0445", -1, 1),
              new Among("\u0435\u044E", -1, 1), new Among("\u043E\u044E", -1, 1),
              new Among("\u0443\u044E", -1, 1), new Among("\u044E\u044E", -1, 1),
              new Among("\u0430\u044F", -1, 1), new Among("\u044F\u044F", -1, 1)
            ],
            a_2 = [
              new Among("\u0435\u043C", -1, 1), new Among("\u043D\u043D", -1, 1),
              new Among("\u0432\u0448", -1, 1),
              new Among("\u0438\u0432\u0448", 2, 2),
              new Among("\u044B\u0432\u0448", 2, 2), new Among("\u0449", -1, 1),
              new Among("\u044E\u0449", 5, 1),
              new Among("\u0443\u044E\u0449", 6, 2)
            ],
            a_3 = [
              new Among("\u0441\u044C", -1, 1), new Among("\u0441\u044F", -1, 1)
            ],
            a_4 = [
              new Among("\u043B\u0430", -1, 1),
              new Among("\u0438\u043B\u0430", 0, 2),
              new Among("\u044B\u043B\u0430", 0, 2),
              new Among("\u043D\u0430", -1, 1),
              new Among("\u0435\u043D\u0430", 3, 2),
              new Among("\u0435\u0442\u0435", -1, 1),
              new Among("\u0438\u0442\u0435", -1, 2),
              new Among("\u0439\u0442\u0435", -1, 1),
              new Among("\u0435\u0439\u0442\u0435", 7, 2),
              new Among("\u0443\u0439\u0442\u0435", 7, 2),
              new Among("\u043B\u0438", -1, 1),
              new Among("\u0438\u043B\u0438", 10, 2),
              new Among("\u044B\u043B\u0438", 10, 2), new Among("\u0439", -1, 1),
              new Among("\u0435\u0439", 13, 2), new Among("\u0443\u0439", 13, 2),
              new Among("\u043B", -1, 1), new Among("\u0438\u043B", 16, 2),
              new Among("\u044B\u043B", 16, 2), new Among("\u0435\u043C", -1, 1),
              new Among("\u0438\u043C", -1, 2), new Among("\u044B\u043C", -1, 2),
              new Among("\u043D", -1, 1), new Among("\u0435\u043D", 22, 2),
              new Among("\u043B\u043E", -1, 1),
              new Among("\u0438\u043B\u043E", 24, 2),
              new Among("\u044B\u043B\u043E", 24, 2),
              new Among("\u043D\u043E", -1, 1),
              new Among("\u0435\u043D\u043E", 27, 2),
              new Among("\u043D\u043D\u043E", 27, 1),
              new Among("\u0435\u0442", -1, 1),
              new Among("\u0443\u0435\u0442", 30, 2),
              new Among("\u0438\u0442", -1, 2), new Among("\u044B\u0442", -1, 2),
              new Among("\u044E\u0442", -1, 1),
              new Among("\u0443\u044E\u0442", 34, 2),
              new Among("\u044F\u0442", -1, 2), new Among("\u043D\u044B", -1, 1),
              new Among("\u0435\u043D\u044B", 37, 2),
              new Among("\u0442\u044C", -1, 1),
              new Among("\u0438\u0442\u044C", 39, 2),
              new Among("\u044B\u0442\u044C", 39, 2),
              new Among("\u0435\u0448\u044C", -1, 1),
              new Among("\u0438\u0448\u044C", -1, 2), new Among("\u044E", -1, 2),
              new Among("\u0443\u044E", 44, 2)
            ],
            a_5 = [
              new Among("\u0430", -1, 1), new Among("\u0435\u0432", -1, 1),
              new Among("\u043E\u0432", -1, 1), new Among("\u0435", -1, 1),
              new Among("\u0438\u0435", 3, 1), new Among("\u044C\u0435", 3, 1),
              new Among("\u0438", -1, 1), new Among("\u0435\u0438", 6, 1),
              new Among("\u0438\u0438", 6, 1),
              new Among("\u0430\u043C\u0438", 6, 1),
              new Among("\u044F\u043C\u0438", 6, 1),
              new Among("\u0438\u044F\u043C\u0438", 10, 1),
              new Among("\u0439", -1, 1), new Among("\u0435\u0439", 12, 1),
              new Among("\u0438\u0435\u0439", 13, 1),
              new Among("\u0438\u0439", 12, 1), new Among("\u043E\u0439", 12, 1),
              new Among("\u0430\u043C", -1, 1), new Among("\u0435\u043C", -1, 1),
              new Among("\u0438\u0435\u043C", 18, 1),
              new Among("\u043E\u043C", -1, 1), new Among("\u044F\u043C", -1, 1),
              new Among("\u0438\u044F\u043C", 21, 1), new Among("\u043E", -1, 1),
              new Among("\u0443", -1, 1), new Among("\u0430\u0445", -1, 1),
              new Among("\u044F\u0445", -1, 1),
              new Among("\u0438\u044F\u0445", 26, 1), new Among("\u044B", -1, 1),
              new Among("\u044C", -1, 1), new Among("\u044E", -1, 1),
              new Among("\u0438\u044E", 30, 1), new Among("\u044C\u044E", 30, 1),
              new Among("\u044F", -1, 1), new Among("\u0438\u044F", 33, 1),
              new Among("\u044C\u044F", 33, 1)
            ],
            a_6 = [
              new Among("\u043E\u0441\u0442", -1, 1),
              new Among("\u043E\u0441\u0442\u044C", -1, 1)
            ],
            a_7 = [
              new Among("\u0435\u0439\u0448\u0435", -1, 1),
              new Among("\u043D", -1, 2), new Among("\u0435\u0439\u0448", -1, 1),
              new Among("\u044C", -1, 3)
            ],
            g_v = [33, 65, 8, 232],
            I_p2, I_pV, sbp = new SnowballProgram();
          this.setCurrent = function(word) {
            sbp.setCurrent(word);
          };
          this.getCurrent = function() {
            return sbp.getCurrent();
          };

          function habr3() {
            while (!sbp.in_grouping(g_v, 1072, 1103)) {
              if (sbp.cursor >= sbp.limit)
                return false;
              sbp.cursor++;
            }
            return true;
          }

          function habr4() {
            while (!sbp.out_grouping(g_v, 1072, 1103)) {
              if (sbp.cursor >= sbp.limit)
                return false;
              sbp.cursor++;
            }
            return true;
          }

          function r_mark_regions() {
            I_pV = sbp.limit;
            I_p2 = I_pV;
            if (habr3()) {
              I_pV = sbp.cursor;
              if (habr4())
                if (habr3())
                  if (habr4())
                    I_p2 = sbp.cursor;
            }
          }

          function r_R2() {
            return I_p2 <= sbp.cursor;
          }

          function habr2(a, n) {
            var among_var, v_1;
            sbp.ket = sbp.cursor;
            among_var = sbp.find_among_b(a, n);
            if (among_var) {
              sbp.bra = sbp.cursor;
              switch (among_var) {
                case 1:
                  v_1 = sbp.limit - sbp.cursor;
                  if (!sbp.eq_s_b(1, "\u0430")) {
                    sbp.cursor = sbp.limit - v_1;
                    if (!sbp.eq_s_b(1, "\u044F"))
                      return false;
                  }
                case 2:
                  sbp.slice_del();
                  break;
              }
              return true;
            }
            return false;
          }

          function r_perfective_gerund() {
            return habr2(a_0, 9);
          }

          function habr1(a, n) {
            var among_var;
            sbp.ket = sbp.cursor;
            among_var = sbp.find_among_b(a, n);
            if (among_var) {
              sbp.bra = sbp.cursor;
              if (among_var == 1)
                sbp.slice_del();
              return true;
            }
            return false;
          }

          function r_adjective() {
            return habr1(a_1, 26);
          }

          function r_adjectival() {
            var among_var;
            if (r_adjective()) {
              habr2(a_2, 8);
              return true;
            }
            return false;
          }

          function r_reflexive() {
            return habr1(a_3, 2);
          }

          function r_verb() {
            return habr2(a_4, 46);
          }

          function r_noun() {
            habr1(a_5, 36);
          }

          function r_derivational() {
            var among_var;
            sbp.ket = sbp.cursor;
            among_var = sbp.find_among_b(a_6, 2);
            if (among_var) {
              sbp.bra = sbp.cursor;
              if (r_R2() && among_var == 1)
                sbp.slice_del();
            }
          }

          function r_tidy_up() {
            var among_var;
            sbp.ket = sbp.cursor;
            among_var = sbp.find_among_b(a_7, 4);
            if (among_var) {
              sbp.bra = sbp.cursor;
              switch (among_var) {
                case 1:
                  sbp.slice_del();
                  sbp.ket = sbp.cursor;
                  if (!sbp.eq_s_b(1, "\u043D"))
                    break;
                  sbp.bra = sbp.cursor;
                case 2:
                  if (!sbp.eq_s_b(1, "\u043D"))
                    break;
                case 3:
                  sbp.slice_del();
                  break;
              }
            }
          }
          this.stem = function() {
            r_mark_regions();
            sbp.cursor = sbp.limit;
            if (sbp.cursor < I_pV)
              return false;
            sbp.limit_backward = I_pV;
            if (!r_perfective_gerund()) {
              sbp.cursor = sbp.limit;
              if (!r_reflexive())
                sbp.cursor = sbp.limit;
              if (!r_adjectival()) {
                sbp.cursor = sbp.limit;
                if (!r_verb()) {
                  sbp.cursor = sbp.limit;
                  r_noun();
                }
              }
            }
            sbp.cursor = sbp.limit;
            sbp.ket = sbp.cursor;
            if (sbp.eq_s_b(1, "\u0438")) {
              sbp.bra = sbp.cursor;
              sbp.slice_del();
            } else
              sbp.cursor = sbp.limit;
            r_derivational();
            sbp.cursor = sbp.limit;
            r_tidy_up();
            return true;
          }
        };

      /* and return a function that stems a word for the current locale */
      return function(word) {
        st.setCurrent(word);
        st.stem();
        return st.getCurrent();
      }
    })();

    lunr.Pipeline.registerFunction(lunr.ru.stemmer, 'stemmer-ru');

    /* stop word filter function */
    lunr.ru.stopWordFilter = function(token) {
      if (lunr.ru.stopWordFilter.stopWords.indexOf(token) === -1) {
        return token;
      }
    };

    lunr.ru.stopWordFilter.stopWords = new lunr.SortedSet();
    lunr.ru.stopWordFilter.stopWords.length = 422;

    // The space at the beginning is crucial: It marks the empty string
    // as a stop word. lunr.js crashes during search when documents
    // processed by the pipeline still contain the empty string.
    lunr.ru.stopWordFilter.stopWords.elements = ' алло без близко более больше будем будет будете будешь будто буду будут будь бы бывает бывь был была были было быть в важная важное важные важный вам вами вас ваш ваша ваше ваши вверх вдали вдруг ведь везде весь вниз внизу во вокруг вон восемнадцатый восемнадцать восемь восьмой вот впрочем времени время все всегда всего всем всеми всему всех всею всю всюду вся всё второй вы г где говорил говорит год года году да давно даже далеко дальше даром два двадцатый двадцать две двенадцатый двенадцать двух девятнадцатый девятнадцать девятый девять действительно дел день десятый десять для до довольно долго должно другая другие других друго другое другой е его ее ей ему если есть еще ещё ею её ж же жизнь за занят занята занято заняты затем зато зачем здесь значит и из или им именно иметь ими имя иногда их к каждая каждое каждые каждый кажется как какая какой кем когда кого ком кому конечно которая которого которой которые который которых кроме кругом кто куда лет ли лишь лучше люди м мало между меля менее меньше меня миллионов мимо мира мне много многочисленная многочисленное многочисленные многочисленный мной мною мог могут мож может можно можхо мои мой мор мочь моя моё мы на наверху над надо назад наиболее наконец нам нами нас начала наш наша наше наши не него недавно недалеко нее ней нельзя нем немного нему непрерывно нередко несколько нет нею неё ни нибудь ниже низко никогда никуда ними них ничего но ну нужно нх о об оба обычно один одиннадцатый одиннадцать однажды однако одного одной около он она они оно опять особенно от отовсюду отсюда очень первый перед по под пожалуйста позже пока пор пора после посреди потом потому почему почти прекрасно при про просто против процентов пятнадцатый пятнадцать пятый пять раз разве рано раньше рядом с сам сама сами самим самими самих само самого самой самом самому саму свое своего своей свои своих свою сеаой себе себя сегодня седьмой сейчас семнадцатый семнадцать семь сих сказал сказала сказать сколько слишком сначала снова со собой собою совсем спасибо стал суть т та так такая также такие такое такой там твой твоя твоё те тебе тебя тем теми теперь тех то тобой тобою тогда того тоже только том тому тот тою третий три тринадцатый тринадцать ту туда тут ты тысяч у уж уже уметь хорошо хотеть хоть хотя хочешь часто чаще чего человек чем чему через четвертый четыре четырнадцатый четырнадцать что чтоб чтобы чуть шестнадцатый шестнадцать шестой шесть эта эти этим этими этих это этого этой этом этому этот эту я ﻿а'.split(' ');

    lunr.Pipeline.registerFunction(lunr.ru.stopWordFilter, 'stopWordFilter-ru');
  };
}));
/**
 * export the module via AMD, CommonJS or as a browser global
 * Export code from https://github.com/umdjs/umd/blob/master/returnExports.js
 */
;(function (root, factory) {
    if (typeof define === 'function' && define.amd) {
        // AMD. Register as an anonymous module.
        define(factory)
    } else if (typeof exports === 'object') {
        /**
         * Node. Does not work with strict CommonJS, but
         * only CommonJS-like environments that support module.exports,
         * like Node.
         */
        module.exports = factory()
    } else {
        // Browser globals (root is window)
        factory()(root.lunr);
    }
}(this, function () {
    /**
     * Just return a value to define the module export.
     * This example returns an object, but the module
     * can return a function as the exported value.
     */
    return function(lunr) {
        /* Set up the pipeline for indexing content in multiple languages. The
           corresponding lunr.{lang} files must be loaded before calling this
           function; English ('en') is built in.

           Returns: a lunr plugin for use in your indexer.

           Known drawback: every word will be stemmed with stemmers for every
           language. This could mean that sometimes words that have the same
           stemming root will not be stemmed as such.
           */
        lunr.multiLanguage = function(/* lang1, lang2, ... */) {
            var languages = Array.prototype.slice.call(arguments);
            var nameSuffix = languages.join('-');
            var wordCharacters = "";
            var pipeline = [];
            for (var i = 0; i < languages.length; ++i) {
                if (languages[i] == 'en') {
                    wordCharacters += '\\w';
                    pipeline.unshift(lunr.stopWordFilter);
                    pipeline.push(lunr.stemmer);
                } else {
                    wordCharacters += lunr[languages[i]].wordCharacters;
                    pipeline.unshift(lunr[languages[i]].stopWordFilter);
                    pipeline.push(lunr[languages[i]].stemmer);
                }
            };
            var multiTrimmer = lunr.trimmerSupport.generateTrimmer(wordCharacters);
            lunr.Pipeline.registerFunction(multiTrimmer, 'lunr-multi-trimmer-' + nameSuffix);
            pipeline.unshift(multiTrimmer);

            return function() {
                this.pipeline.reset();
                this.pipeline.add.apply(this.pipeline, pipeline);
            };
        }
    }
}));

window.search = window.search || {};
(function search(search) {
    // Search functionality
    //
    // You can use !hasFocus() to prevent keyhandling in your key
    // event handlers while the user is typing their search.

    if (!Mark || !elasticlunr) {
        return;
    }

    //IE 11 Compatibility from https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/String/startsWith
    if (!String.prototype.startsWith) {
        String.prototype.startsWith = function(search, pos) {
            return this.substr(!pos || pos < 0 ? 0 : +pos, search.length) === search;
        };
    }

    var search_wrap = document.getElementById('search-wrapper'),
        searchbar = document.getElementById('searchbar'),
        searchbar_outer = document.getElementById('searchbar-outer'),
        searchresults = document.getElementById('searchresults'),
        searchresults_outer = document.getElementById('searchresults-outer'),
        searchresults_header = document.getElementById('searchresults-header'),
        searchicon = document.getElementById('search-toggle'),
        content = document.getElementById('content'),

        searchindex = null,
        doc_urls = [],
        results_options = {
            teaser_word_count: 30,
            limit_results: 30,
        },
        search_options = {
            bool: "AND",
            expand: true,
            fields: {
                title: {boost: 1},
                body: {boost: 1},
                breadcrumbs: {boost: 0}
            }
        },
        mark_exclude = [],
        marker = new Mark(content),
        current_searchterm = "",
        URL_SEARCH_PARAM = 'search',
        URL_MARK_PARAM = 'highlight',
        teaser_count = 0,

        SEARCH_HOTKEY_KEYCODE = 70,
        ESCAPE_KEYCODE = 27,
        DOWN_KEYCODE = 40,
        UP_KEYCODE = 38,
        SELECT_KEYCODE = 13;

    function hasFocus() {
        return searchbar === document.activeElement;
    }

    function removeChildren(elem) {
        while (elem.firstChild) {
            elem.removeChild(elem.firstChild);
        }
    }

    // Helper to parse a url into its building blocks.
    function parseURL(url) {
        var a =  document.createElement('a');
        a.href = url;
        return {
            source: url,
            protocol: a.protocol.replace(':',''),
            host: a.hostname,
            port: a.port,
            params: (function(){
                var ret = {};
                var seg = a.search.replace(/^\?/,'').split('&');
                var len = seg.length, i = 0, s;
                for (;i<len;i++) {
                    if (!seg[i]) { continue; }
                    s = seg[i].split('=');
                    ret[s[0]] = s[1];
                }
                return ret;
            })(),
            file: (a.pathname.match(/\/([^/?#]+)$/i) || [,''])[1],
            hash: a.hash.replace('#',''),
            path: a.pathname.replace(/^([^/])/,'/$1')
        };
    }
    
    // Helper to recreate a url string from its building blocks.
    function renderURL(urlobject) {
        var url = urlobject.protocol + "://" + urlobject.host;
        if (urlobject.port != "") {
            url += ":" + urlobject.port;
        }
        url += urlobject.path;
        var joiner = "?";
        for(var prop in urlobject.params) {
            if(urlobject.params.hasOwnProperty(prop)) {
                url += joiner + prop + "=" + urlobject.params[prop];
                joiner = "&";
            }
        }
        if (urlobject.hash != "") {
            url += "#" + urlobject.hash;
        }
        return url;
    }
    
    // Helper to escape html special chars for displaying the teasers
    var escapeHTML = (function() {
        var MAP = {
            '&': '&amp;',
            '<': '&lt;',
            '>': '&gt;',
            '"': '&#34;',
            "'": '&#39;'
        };
        var repl = function(c) { return MAP[c]; };
        return function(s) {
            return s.replace(/[&<>'"]/g, repl);
        };
    })();
    
    function formatSearchMetric(count, searchterm) {
        if (count == 1) {
            return count + " найдено для '" + searchterm + "':";
        } else if (count == 0) {
            return "Ничего не найдено для '" + searchterm + "'.";
        } else {
            return count + " найдено для '" + searchterm + "':";
        }
    }
    
    function formatSearchResult(result, searchterms) {
        var teaser = makeTeaser(escapeHTML(result.doc.body), searchterms);
        teaser_count++;

        // The ?URL_MARK_PARAM= parameter belongs inbetween the page and the #heading-anchor
        var url = doc_urls[result.ref].split("#");
        if (url.length == 1) { // no anchor found
            url.push("");
        }

        // encodeURIComponent escapes all chars that could allow an XSS except
        // for '. Due to that we also manually replace ' with its url-encoded
        // representation (%27).
        var searchterms = encodeURIComponent(searchterms.join(" ")).replace(/\'/g, "%27");

        return '<a href="' + path_to_root + url[0] + '?' + URL_MARK_PARAM + '=' + searchterms + '#' + url[1]
            + '" aria-details="teaser_' + teaser_count + '">' + result.doc.breadcrumbs + '</a>'
            + '<span class="teaser" id="teaser_' + teaser_count + '" aria-label="Search Result Teaser">' 
            + teaser + '</span>';
    }
    
    function makeTeaser(body, searchterms) {
        // The strategy is as follows:
        // First, assign a value to each word in the document:
        //  Words that correspond to search terms (stemmer aware): 40
        //  Normal words: 2
        //  First word in a sentence: 8
        // Then use a sliding window with a constant number of words and count the
        // sum of the values of the words within the window. Then use the window that got the
        // maximum sum. If there are multiple maximas, then get the last one.
        // Enclose the terms in <em>.
        var stemmed_searchterms = searchterms.map(function(w) {
            return elasticlunr.stemmer(w.toLowerCase());
        });
        var searchterm_weight = 40;
        var weighted = []; // contains elements of ["word", weight, index_in_document]
        // split in sentences, then words
        var sentences = body.toLowerCase().split('. ');
        var index = 0;
        var value = 0;
        var searchterm_found = false;
        for (var sentenceindex in sentences) {
            var words = sentences[sentenceindex].split(' ');
            value = 8;
            for (var wordindex in words) {
                var word = words[wordindex];
                if (word.length > 0) {
                    for (var searchtermindex in stemmed_searchterms) {
                        if (elasticlunr.stemmer(word).startsWith(stemmed_searchterms[searchtermindex])) {
                            value = searchterm_weight;
                            searchterm_found = true;
                        }
                    };
                    weighted.push([word, value, index]);
                    value = 2;
                }
                index += word.length;
                index += 1; // ' ' or '.' if last word in sentence
            };
            index += 1; // because we split at a two-char boundary '. '
        };

        if (weighted.length == 0) {
            return body;
        }

        var window_weight = [];
        var window_size = Math.min(weighted.length, results_options.teaser_word_count);

        var cur_sum = 0;
        for (var wordindex = 0; wordindex < window_size; wordindex++) {
            cur_sum += weighted[wordindex][1];
        };
        window_weight.push(cur_sum);
        for (var wordindex = 0; wordindex < weighted.length - window_size; wordindex++) {
            cur_sum -= weighted[wordindex][1];
            cur_sum += weighted[wordindex + window_size][1];
            window_weight.push(cur_sum);
        };

        if (searchterm_found) {
            var max_sum = 0;
            var max_sum_window_index = 0;
            // backwards
            for (var i = window_weight.length - 1; i >= 0; i--) {
                if (window_weight[i] > max_sum) {
                    max_sum = window_weight[i];
                    max_sum_window_index = i;
                }
            };
        } else {
            max_sum_window_index = 0;
        }

        // add <em/> around searchterms
        var teaser_split = [];
        var index = weighted[max_sum_window_index][2];
        for (var i = max_sum_window_index; i < max_sum_window_index+window_size; i++) {
            var word = weighted[i];
            if (index < word[2]) {
                // missing text from index to start of `word`
                teaser_split.push(body.substring(index, word[2]));
                index = word[2];
            }
            if (word[1] == searchterm_weight) {
                teaser_split.push("<em>")
            }
            index = word[2] + word[0].length;
            teaser_split.push(body.substring(word[2], index));
            if (word[1] == searchterm_weight) {
                teaser_split.push("</em>")
            }
        };

        return teaser_split.join('');
    }

    function init(config) {
        results_options = config.results_options;
        search_options = config.search_options;
        searchbar_outer = config.searchbar_outer;
        doc_urls = config.doc_urls;

        //searchindex = elasticlunr.Index.load(config.index);

        searchindex = elasticlunr(function() {
            this.use(elasticlunr.multiLanguage('en', 'ru'));
            // Set fields to index.
            this.addField('title');
            this.addField('body');
            this.addField('breadcrumbs');
            // Set the field used to identify documents.
            this.setRef('id');
            
            for (let key in config.docs) {
              this.addDoc(config.docs[key]);
            }
        });

        searchindex.use(elasticlunr.multiLanguage('en', 'ru'));

        window.lunrsearch = searchindex;

        // Set up events
        searchicon.addEventListener('click', function(e) { searchIconClickHandler(); }, false);
        searchbar.addEventListener('keyup', function(e) { searchbarKeyUpHandler(); }, false);
        document.addEventListener('keydown', function(e) { globalKeyHandler(e); }, false);
        // If the user uses the browser buttons, do the same as if a reload happened
        window.onpopstate = function(e) { doSearchOrMarkFromUrl(); };
        // Suppress "submit" events so the page doesn't reload when the user presses Enter
        document.addEventListener('submit', function(e) { e.preventDefault(); }, false);

        // If reloaded, do the search or mark again, depending on the current url parameters
        doSearchOrMarkFromUrl();
    }
    
    function unfocusSearchbar() {
        // hacky, but just focusing a div only works once
        var tmp = document.createElement('input');
        tmp.setAttribute('style', 'position: absolute; opacity: 0;');
        searchicon.appendChild(tmp);
        tmp.focus();
        tmp.remove();
    }
    
    // On reload or browser history backwards/forwards events, parse the url and do search or mark
    function doSearchOrMarkFromUrl() {
        // Check current URL for search request
        var url = parseURL(window.location.href);
        if (url.params.hasOwnProperty(URL_SEARCH_PARAM)
            && url.params[URL_SEARCH_PARAM] != "") {
            showSearch(true);
            searchbar.value = decodeURIComponent(
                (url.params[URL_SEARCH_PARAM]+'').replace(/\+/g, '%20'));
            searchbarKeyUpHandler(); // -> doSearch()
        } else {
            showSearch(false);
        }

        if (url.params.hasOwnProperty(URL_MARK_PARAM)) {
            var words = decodeURIComponent(url.params[URL_MARK_PARAM]).split(' ');
            marker.mark(words, {
                exclude: mark_exclude
            });

            var markers = document.querySelectorAll("mark");
            function hide() {
                for (var i = 0; i < markers.length; i++) {
                    markers[i].classList.add("fade-out");
                    window.setTimeout(function(e) { marker.unmark(); }, 300);
                }
            }
            for (var i = 0; i < markers.length; i++) {
                markers[i].addEventListener('click', hide);
            }
        }
    }
    
    // Eventhandler for keyevents on `document`
    function globalKeyHandler(e) {
        if (e.altKey || e.metaKey || e.shiftKey || e.target.type === 'textarea' || e.target.type === 'text' || !hasFocus() && /^(?:input|select|textarea)$/i.test(e.target.nodeName)) { return; }

        function esc() {
            searchbar.classList.remove("active");
            setSearchUrlParameters("",
                (searchbar.value.trim() !== "") ? "push" : "replace");
            if (hasFocus()) {
                unfocusSearchbar();
            }
            showSearch(false);
            marker.unmark();
        }

        if (e.keyCode === ESCAPE_KEYCODE) {
            e.preventDefault();
            esc();
        } else if (!hasFocus() && e.keyCode === SEARCH_HOTKEY_KEYCODE && e.ctrlKey) {
            e.preventDefault();
            if (!search_wrap.classList.contains('hidden')) {
                esc();
            } else {
                showSearch(true);
                window.scrollTo(0, 0);
                searchbar.select();
            }
        } else if (hasFocus() && e.keyCode === DOWN_KEYCODE) {
            e.preventDefault();
            unfocusSearchbar();
            searchresults.firstElementChild.classList.add("focus");
        } else if (!hasFocus() && (e.keyCode === DOWN_KEYCODE
                                || e.keyCode === UP_KEYCODE
                                || e.keyCode === SELECT_KEYCODE)) {
            // not `:focus` because browser does annoying scrolling
            var focused = searchresults.querySelector("li.focus");
            if (!focused) return;
            e.preventDefault();
            if (e.keyCode === DOWN_KEYCODE) {
                var next = focused.nextElementSibling;
                if (next) {
                    focused.classList.remove("focus");
                    next.classList.add("focus");
                }
            } else if (e.keyCode === UP_KEYCODE) {
                focused.classList.remove("focus");
                var prev = focused.previousElementSibling;
                if (prev) {
                    prev.classList.add("focus");
                } else {
                    searchbar.select();
                }
            } else { // SELECT_KEYCODE
                window.location.assign(focused.querySelector('a'));
            }
        }
    }
    
    function showSearch(yes) {
        if (yes) {
            search_wrap.classList.remove('hidden');
            searchicon.setAttribute('aria-expanded', 'true');
        } else {
            search_wrap.classList.add('hidden');
            searchicon.setAttribute('aria-expanded', 'false');
            var results = searchresults.children;
            for (var i = 0; i < results.length; i++) {
                results[i].classList.remove("focus");
            }
        }
    }

    function showResults(yes) {
        if (yes) {
            searchresults_outer.classList.remove('hidden');
        } else {
            searchresults_outer.classList.add('hidden');
        }
    }

    // Eventhandler for search icon
    function searchIconClickHandler() {
        if (search_wrap.classList.contains('hidden')) {
            showSearch(true);
            window.scrollTo(0, 0);
            searchbar.select();
        } else {
            showSearch(false);
        }
    }
    
    // Eventhandler for keyevents while the searchbar is focused
    function searchbarKeyUpHandler() {
        var searchterm = searchbar.value.trim();
        if (searchterm != "") {
            searchbar.classList.add("active");
            doSearch(searchterm);
        } else {
            searchbar.classList.remove("active");
            showResults(false);
            removeChildren(searchresults);
        }

        setSearchUrlParameters(searchterm, "push_if_new_search_else_replace");

        // Remove marks
        marker.unmark();
    }
    
    // Update current url with ?URL_SEARCH_PARAM= parameter, remove ?URL_MARK_PARAM and #heading-anchor .
    // `action` can be one of "push", "replace", "push_if_new_search_else_replace"
    // and replaces or pushes a new browser history item.
    // "push_if_new_search_else_replace" pushes if there is no `?URL_SEARCH_PARAM=abc` yet.
    function setSearchUrlParameters(searchterm, action) {
        var url = parseURL(window.location.href);
        var first_search = ! url.params.hasOwnProperty(URL_SEARCH_PARAM);
        if (searchterm != "" || action == "push_if_new_search_else_replace") {
            url.params[URL_SEARCH_PARAM] = searchterm;
            delete url.params[URL_MARK_PARAM];
            url.hash = "";
        } else {
            delete url.params[URL_MARK_PARAM];
            delete url.params[URL_SEARCH_PARAM];
        }
        // A new search will also add a new history item, so the user can go back
        // to the page prior to searching. A updated search term will only replace
        // the url.
        if (action == "push" || (action == "push_if_new_search_else_replace" && first_search) ) {
            history.pushState({}, document.title, renderURL(url));
        } else if (action == "replace" || (action == "push_if_new_search_else_replace" && !first_search) ) {
            history.replaceState({}, document.title, renderURL(url));
        }
    }
    
    function doSearch(searchterm) {

        // Don't search the same twice
        if (current_searchterm == searchterm) { return; }
        else { current_searchterm = searchterm; }

        if (searchindex == null) { return; }

        // Do the actual search
        var results = searchindex.search(searchterm, search_options);
        var resultcount = Math.min(results.length, results_options.limit_results);

        // Display search metrics
        searchresults_header.innerText = formatSearchMetric(resultcount, searchterm);

        // Clear and insert results
        var searchterms  = searchterm.split(' ');
        removeChildren(searchresults);
        for(var i = 0; i < resultcount ; i++){
            var resultElem = document.createElement('li');
            resultElem.innerHTML = formatSearchResult(results[i], searchterms);
            searchresults.appendChild(resultElem);
        }

        // Display results
        showResults(true);
    }

    //fetch(path_to_root + 'searchindex.json')
    //    .then(response => response.json())
    //    .then(json => init(json))
    //    .catch(error => { // Try to load searchindex.js if fetch failed
    //        var script = document.createElement('script');
    //        script.src = path_to_root + 'searchindex.js';
    //        script.onload = () => init(window.search);
    //        document.head.appendChild(script);
    //    });

    init(window.search);

    // Exported functions
    search.hasFocus = hasFocus;
})(window.search);
