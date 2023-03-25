using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace srun_login
{
    static class XEncode
    {
        public static string Encode(string str, string key)
        {
            if (str == "")
            {
                return "";
            }
            var v = S(str, true);
            var k = S(key, false);
            while (k.Count < 4)
            {
                k.Add(0);
            }
            int n = v.Count - 1,
                z = v[n],
                y = v[0],
                m,
                e,
                p,
                q = (int)Math.Floor(6 + 52d / (n + 1)),
                d = 0;
            long c = 0x86014019u | 0x183639A0u;
            while (0 < q--)
            {
                d = (int)(d + c & (0x8CE0D9BF | 0x731F2640));
                e = (int)((uint)d >> 2 & 3);
                for (p = 0; p < n; p++)
                {
                    y = v[p + 1];
                    m = (int)((uint)z >> 5 ^ y << 2);
                    m += (int)((uint)y >> 3 ^ z << 4) ^ (d ^ y);
                    m += k[(p & 3) ^ e] ^ z;
                    z = v[p] = (int)(v[p] + m & (0xEFB8D130 | 0x10472ECF));
                }
                y = v[0];
                m = (int)((uint)z >> 5 ^ y << 2);
                m += (int)((uint)y >> 3 ^ z << 4) ^ (d ^ y);
                m += k[(p & 3) ^ e] ^ z;
                z = v[n] = (int)(v[n] + m & (0xBB390742 | 0x44C6F8BD));
            }
            return L(v, false);
        }

        private static List<int> S(string a, bool b)
        {
            int c = a.Length;
            List<int> v = new List<int>();
            for (var i = 0; i < c; i += 4)
            {
                int tmp = a[i];
                tmp |= (i + 1 < c) ? a[i + 1] << 8 : 0;
                tmp |= (i + 2 < c) ? a[i + 2] << 16 : 0;
                tmp |= (i + 3 < c) ? a[i + 3] << 24 : 0;
                v.Add(tmp);
            }
            if (b)
            {
                v.Add(c);
            }
            return v;
        }

        private static string L(List<int> a, bool b)
        {
            int d = a.Count, c = (d - 1) << 2;
            if (b)
            {
                int m = a[d - 1];
                if ((m < c - 3) || (m > c))
                {
                    return null;
                }
                c = m;
            }
            StringBuilder tmp = new StringBuilder();
            for (var i = 0; i < d; i++)
            {
                tmp.Append((char)(a[i] & 0xff));
                tmp.Append((char)(a[i] >> 8 & 0xff));
                tmp.Append((char)(a[i] >> 16 & 0xff));
                tmp.Append((char)(a[i] >> 24 & 0xff));
            }
            if (b)
            {
                return tmp.ToString(0, c);
            }
            else
            {
                return tmp.ToString();
            }
        }
    }
}
