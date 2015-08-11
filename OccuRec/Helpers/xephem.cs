using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OccuRec.Helpers
{
    class xephem
    {
        //private static double last_mjd, last_dy;
        //private static int last_mn, last_yr;

        // number of days elapsed since 1900 jan 0.5
        public static double GetMJD(DateTime moment)
        {
            // Calc 1900 jan 0.5 as 1900 jan 1.5 minus 1 day
            DateTime ZERO = (new DateTime(1900, 1, 1, 12, 0, 0)).AddDays(-1);

            TimeSpan ts = new TimeSpan(moment.Ticks - ZERO.Ticks);
            return ts.TotalDays;
        }

        public static DateTime GetDateFromMJD(double mjd)
        {
            return (new DateTime(1900, 1, 1, 12, 0, 0)).AddDays(mjd);
        }

        public static double JD(DateTime moment)
        {
            return JD(moment.Year, moment.Month, ((double)moment.Day + ((double)moment.Hour + (double)moment.Minute / 60 + (double)moment.Second / 3600) / 24));
        }

        public static double JD(int y, int m, double dy)
        {
            int[] monthday = new int[12] { 0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334 };

            double p = 0;
            int intjd;

            switch (y % 4)
            {
                case 1:
                    {
                        p = 1.75;
                        break;
                    };
                case 2:
                    {
                        p = 1.5;
                        break;
                    };
                case 3:
                    {
                        p = 1.25;
                        break;
                    };
                case 0:
                    {
                        p = 1.0;
                        break;
                    };
            };

            if (p == 1 && m > 2) p += 1;
            intjd = (y / 400) - (y / 100);
            return 365.25 * (4712.0 + y) + intjd + p - 0.5 + monthday[m - 1] + dy;
        }

        private static double degrad(double x)
        {
            return (x) * System.Math.PI / 180.0;
        }

        private static double raddeg(double x)
        {
            return (x) * 180.0 / System.Math.PI;
        }

        public static void range(ref double x, double r)
        {
            x -= r * System.Math.Floor(x / r);
        }


        private static void obliquity(double mjd, out double eps)
        {
            double t;
            t = mjd / 36525.0;
            eps = degrad(2.345229444E1 - ((((-1.81E-3 * t) + 5.9E-3) * t + 4.6845E1) * t) / 3600.0);
        }

        private static void nutation(double mjd, out double deps, out double dpsi)
        {
            double ls, ld;	/* sun's mean longitude, moon's mean longitude */
            double ms, md;	/* sun's mean anomaly, moon's mean anomaly */
            double nm;	/* longitude of moon's ascending node */
            double t, t2;	/* number of Julian centuries of 36525 days since
			         * Jan 0.5 1900.
			         */
            double tls, tnm, tld;	/* twice above */
            double a, b;	/* temps */

            t = mjd / 36525.0;
            t2 = t * t;

            a = 100.0021358 * t;
            b = 360.0 * (a - (long)a);
            ls = 279.697 + .000303 * t2 + b;

            a = 1336.855231 * t;
            b = 360.0 * (a - (long)a);
            ld = 270.434 - .001133 * t2 + b;

            a = 99.99736056000026 * t;
            b = 360.0 * (a - (long)a);
            ms = 358.476 - .00015 * t2 + b;

            a = 13255523.59 * t;
            b = 360.0 * (a - (long)a);
            md = 296.105 + .009192 * t2 + b;

            a = 5.372616667 * t;
            b = 360.0 * (a - (long)a);
            nm = 259.183 + .002078 * t2 - b;

            /* convert to radian forms for use with trig functions.
             */
            tls = 2 * degrad(ls);
            nm = degrad(nm);
            tnm = 2 * nm;
            ms = degrad(ms);
            tld = 2 * degrad(ld);
            md = degrad(md);

            /* find delta psi and eps, in arcseconds.
             */
            dpsi = (-17.2327 - .01737 * t) * System.Math.Sin(nm) + (-1.2729 - .00013 * t) * System.Math.Sin(tls)
                   + .2088 * System.Math.Sin(tnm) - .2037 * System.Math.Sin(tld) + (.1261 - .00031 * t) * System.Math.Sin(ms)
                   + .0675 * System.Math.Sin(md) - (.0497 - .00012 * t) * System.Math.Sin(tls + ms)
                   - .0342 * System.Math.Sin(tld - nm) - .0261 * System.Math.Sin(tld + md) + .0214 * System.Math.Sin(tls - ms)
                   - .0149 * System.Math.Sin(tls - tld + md) + .0124 * System.Math.Sin(tls - nm) + .0114 * System.Math.Sin(tld - md);
            deps = (9.21 + .00091 * t) * System.Math.Cos(nm) + (.5522 - .00029 * t) * System.Math.Cos(tls)
                   - .0904 * System.Math.Cos(tnm) + .0884 * System.Math.Cos(tld) + .0216 * System.Math.Cos(tls + ms)
                   + .0183 * System.Math.Cos(tld - nm) + .0113 * System.Math.Cos(tld + md) - .0093 * System.Math.Cos(tls - ms)
                   - .0066 * System.Math.Cos(tls - nm);

            /* convert to radians.
             */
            dpsi = degrad(dpsi / 3600);
            deps = degrad(deps / 3600);
        }

        public static void Precession(int StartEquinox, double JDEnd, ref double RA, ref double Dec, double muRA, double muDec)
        {
            if (StartEquinox == 1950)
            {
                RA = RA + 50.0 * muRA;
                Dec = Dec + 50.0 * muDec;
                RA = RA + 0.0055878714;
                double y = Math.Cos(Dec) * Math.Sin(RA);
                double x = Math.Cos(0.0048580348) * Math.Cos(Dec) * Math.Cos(RA) - Math.Sin(0.0048580348) * Math.Sin(Dec);
                double num = Math.Cos(0.0048580348) * Math.Sin(Dec) + Math.Sin(0.0048580348) * Math.Cos(Dec) * Math.Cos(RA);
                RA = Math.Atan2(y, x) + 0.0055888302;
                Dec = Math.Atan(num / Math.Sqrt(y * y + x * x));
                muRA = muRA + 0.0048581 * (muRA * Math.Cos(RA) * Math.Tan(Dec) + muDec * Math.Sin(RA) / Math.Cos(Dec) / Math.Cos(Dec)) - 2.114E-08 * Math.Sin(RA) * Math.Tan(Dec) + 6.7E-09;
                muDec = muDec - 0.0048581 * muRA * Math.Sin(RA) - 2.114E-08 * Math.Cos(RA);
                RA = RA + 1.651E-06 * Math.Sin(RA + 2.945) / Math.Cos(Dec) + 5.636E-06;
                Dec = Dec + 1.653E-06 * Math.Cos(RA + 2.945) * Math.Sin(Dec) + 1.406E-07 * Math.Cos(Dec);
                if (RA < 0.0)
                    RA += 2.0 * Math.PI;
                if (RA > 2.0 * Math.PI)
                    RA -= 2.0 * Math.PI;
            }
            double num1 = (JDEnd - 2451545.0) / 36525.0;
            if (num1 == 0.0)
                return;
            double num2 = (2.6505 + 1441302.0 / 625.0 * num1 + 0.29885 * num1 * num1 + 0.01802 * num1 * num1 * num1) / 57.2957795130823 / 3600.0;
            double num3 = (2306.0772 * num1 - 2.6505 + 1.09273 * num1 * num1 + 0.01827 * num1 * num1 * num1) / 57.2957795130823 / 3600.0;
            double num4 = (2004.1919 * num1 - 0.42949 * num1 * num1 - 0.04182 * num1 * num1 * num1) / 57.2957795130823 / 3600.0;
            RA = RA + muRA * num1 * 100.0;
            Dec = Dec + muDec * num1 * 100.0;
            RA = RA + num2;
            double y1 = Math.Cos(Dec) * Math.Sin(RA);
            double x1 = Math.Cos(num4) * Math.Cos(Dec) * Math.Cos(RA) - Math.Sin(num4) * Math.Sin(Dec);
            double num5 = Math.Cos(num4) * Math.Sin(Dec) + Math.Sin(num4) * Math.Cos(Dec) * Math.Cos(RA);
            RA = Math.Atan2(y1, x1) + num3;
            if (RA < 0.0)
                RA += 2.0 * Math.PI;
            if (RA > 2.0 * Math.PI)
                RA -= 2.0 * Math.PI;
            Dec = Math.Atan(num5 / Math.Sqrt(y1 * y1 + x1 * x1));
        }

        public static void GetMoonPosition(DateTime moment, out double RA, out double DEC)
        {
            double mjd = GetMJD(moment);
            double lam, bet, hp;
            SolveMoon(mjd, out lam, out bet, out hp);
            ecleq_aux(ECLtoEQ, mjd, lam, bet, out RA, out DEC);
            if (DEC > System.Math.PI) DEC -= 2 * System.Math.PI;

            RA = raddeg(RA);
            DEC = raddeg(DEC);
        }

        public static void GetSunPosition(DateTime moment, out double RA, out double DEC)
        {
            double mjd = GetMJD(moment);
            double l, r;
            sunpos(mjd, out l, out r);
            ecleq_aux(ECLtoEQ, mjd, l, 0, out RA, out DEC);
            if (DEC > System.Math.PI) DEC -= 2 * System.Math.PI;

            RA = raddeg(RA);
            DEC = raddeg(DEC);
        }


        private const int EQtoECL = 1;
        private const int ECLtoEQ = -1;

        public static void SolveMoon(double mjd, out double lam, out double bet, out double hp)
        {
            double t, t2;
            double ld;
            double ms;
            double md;
            double de;
            double f;
            double n;
            double a, sa, sn, b, sb, c, sc, e, e2, l, g, w1, w2;
            double m1, m2, m3, m4, m5, m6;

            t = mjd / 36525.0;
            t2 = t * t;

            m1 = mjd / 27.32158213;
            m1 = 360.0 * (m1 - (long)m1);
            m2 = mjd / 365.2596407;
            m2 = 360.0 * (m2 - (long)m2);
            m3 = mjd / 27.55455094;
            m3 = 360.0 * (m3 - (long)m3);
            m4 = mjd / 29.53058868;
            m4 = 360.0 * (m4 - (long)m4);
            m5 = mjd / 27.21222039;
            m5 = 360.0 * (m5 - (long)m5);
            m6 = mjd / 6798.363307;
            m6 = 360.0 * (m6 - (long)m6);

            ld = 270.434164 + m1 - (.001133 - .0000019 * t) * t2;
            ms = 358.475833 + m2 - (.00015 + .0000033 * t) * t2;
            md = 296.104608 + m3 + (.009192 + .0000144 * t) * t2;
            de = 350.737486 + m4 - (.001436 - .0000019 * t) * t2;
            f = 11.250889 + m5 - (.003211 + .0000003 * t) * t2;
            n = 259.183275 - m6 + (.002078 + .000022 * t) * t2;

            a = degrad(51.2 + 20.2 * t);
            sa = System.Math.Sin(a);
            sn = System.Math.Sin(degrad(n));
            b = 346.56 + (132.87 - .0091731 * t) * t;
            sb = .003964 * System.Math.Sin(degrad(b));
            c = degrad(n + 275.05 - 2.3 * t);
            sc = System.Math.Sin(c);
            ld = ld + .000233 * sa + sb + .001964 * sn;
            ms = ms - .001778 * sa;
            md = md + .000817 * sa + sb + .002541 * sn;
            f = f + sb - .024691 * sn - .004328 * sc;
            de = de + .002011 * sa + sb + .001964 * sn;
            e = 1 - (.002495 + 7.52e-06 * t) * t;
            e2 = e * e;

            ld = degrad(ld);
            ms = degrad(ms);
            n = degrad(n);
            de = degrad(de);
            f = degrad(f);
            md = degrad(md);

            l = 6.28875 * System.Math.Sin(md) + 1.27402 * System.Math.Sin(2 * de - md) + .658309 * System.Math.Sin(2 * de) +
                .213616 * System.Math.Sin(2 * md) - e * .185596 * System.Math.Sin(ms) - .114336 * System.Math.Sin(2 * f) +
                .058793 * System.Math.Sin(2 * (de - md)) + .057212 * e * System.Math.Sin(2 * de - ms - md) +
                .05332 * System.Math.Sin(2 * de + md) + .045874 * e * System.Math.Sin(2 * de - ms) + .041024 * e * System.Math.Sin(md - ms);
            l = l - .034718 * System.Math.Sin(de) - e * .030465 * System.Math.Sin(ms + md) + .015326 * System.Math.Sin(2 * (de - f)) -
                .012528 * System.Math.Sin(2 * f + md) - .01098 * System.Math.Sin(2 * f - md) + .010674 * System.Math.Sin(4 * de - md) +
                .010034 * System.Math.Sin(3 * md) + .008548 * System.Math.Sin(4 * de - 2 * md) - e * .00791 * System.Math.Sin(ms - md + 2 * de) -
                e * .006783 * System.Math.Sin(2 * de + ms);
            l = l + .005162 * System.Math.Sin(md - de) + e * .005 * System.Math.Sin(ms + de) + .003862 * System.Math.Sin(4 * de) +
                e * .004049 * System.Math.Sin(md - ms + 2 * de) + .003996 * System.Math.Sin(2 * (md + de)) +
                .003665 * System.Math.Sin(2 * de - 3 * md) + e * .002695 * System.Math.Sin(2 * md - ms) +
                .002602 * System.Math.Sin(md - 2 * (f + de)) + e * .002396 * System.Math.Sin(2 * (de - md) - ms) -
                .002349 * System.Math.Sin(md + de);
            l = l + e2 * .002249 * System.Math.Sin(2 * (de - ms)) - e * .002125 * System.Math.Sin(2 * md + ms) -
                e2 * .002079 * System.Math.Sin(2 * ms) + e2 * .002059 * System.Math.Sin(2 * (de - ms) - md) -
                .001773 * System.Math.Sin(md + 2 * (de - f)) - .001595 * System.Math.Sin(2 * (f + de)) +
                e * .00122 * System.Math.Sin(4 * de - ms - md) - .00111 * System.Math.Sin(2 * (md + f)) + .000892 * System.Math.Sin(md - 3 * de);
            l = l - e * .000811 * System.Math.Sin(ms + md + 2 * de) + e * .000761 * System.Math.Sin(4 * de - ms - 2 * md) +
                 e2 * .000704 * System.Math.Sin(md - 2 * (ms + de)) + e * .000693 * System.Math.Sin(ms - 2 * (md - de)) +
                 e * .000598 * System.Math.Sin(2 * (de - f) - ms) + .00055 * System.Math.Sin(md + 4 * de) + .000538 * System.Math.Sin(4 * md) +
                 e * .000521 * System.Math.Sin(4 * de - ms) + .000486 * System.Math.Sin(2 * md - de);
            l = l + e2 * .000717 * System.Math.Sin(md - 2 * ms);

            lam = ld + degrad(l);
            range(ref lam, 2 * System.Math.PI);

            g = 5.12819 * System.Math.Sin(f) + .280606 * System.Math.Sin(md + f) + .277693 * System.Math.Sin(md - f) +
                .173238 * System.Math.Sin(2 * de - f) + .055413 * System.Math.Sin(2 * de + f - md) + .046272 * System.Math.Sin(2 * de - f - md) +
                .032573 * System.Math.Sin(2 * de + f) + .017198 * System.Math.Sin(2 * md + f) + .009267 * System.Math.Sin(2 * de + md - f) +
                .008823 * System.Math.Sin(2 * md - f) + e * .008247 * System.Math.Sin(2 * de - ms - f);
            g = g + .004323 * System.Math.Sin(2 * (de - md) - f) + .0042 * System.Math.Sin(2 * de + f + md) +
                e * .003372 * System.Math.Sin(f - ms - 2 * de) + e * .002472 * System.Math.Sin(2 * de + f - ms - md) +
                e * .002222 * System.Math.Sin(2 * de + f - ms) + e * .002072 * System.Math.Sin(2 * de - f - ms - md) +
                e * .001877 * System.Math.Sin(f - ms + md) + .001828 * System.Math.Sin(4 * de - f - md) - e * .001803 * System.Math.Sin(f + ms) -
                .00175 * System.Math.Sin(3 * f);
            g = g + e * .00157 * System.Math.Sin(md - ms - f) - .001487 * System.Math.Sin(f + de) - e * .001481 * System.Math.Sin(f + ms + md) +
                 e * .001417 * System.Math.Sin(f - ms - md) + e * .00135 * System.Math.Sin(f - ms) + .00133 * System.Math.Sin(f - de) +
                 .001106 * System.Math.Sin(f + 3 * md) + .00102 * System.Math.Sin(4 * de - f) + .000833 * System.Math.Sin(f + 4 * de - md) +
                 .000781 * System.Math.Sin(md - 3 * f) + .00067 * System.Math.Sin(f + 4 * de - 2 * md);
            g = g + .000606 * System.Math.Sin(2 * de - 3 * f) + .000597 * System.Math.Sin(2 * (de + md) - f) +
                e * .000492 * System.Math.Sin(2 * de + md - ms - f) + .00045 * System.Math.Sin(2 * (md - de) - f) +
                .000439 * System.Math.Sin(3 * md - f) + .000423 * System.Math.Sin(f + 2 * (de + md)) +
                .000422 * System.Math.Sin(2 * de - f - 3 * md) - e * .000367 * System.Math.Sin(ms + f + 2 * de - md) -
                e * .000353 * System.Math.Sin(ms + f + 2 * de) + .000331 * System.Math.Sin(f + 4 * de);
            g = g + e * .000317 * System.Math.Sin(2 * de + f - ms + md) + e2 * .000306 * System.Math.Sin(2 * (de - ms) - f) -
                .000283 * System.Math.Sin(md + 3 * f);
            w1 = .0004664 * System.Math.Cos(n);
            w2 = .0000754 * System.Math.Cos(c);
            bet = degrad(g) * (1 - w1 - w2);

            hp = .950724 + .051818 * System.Math.Cos(md) + .009531 * System.Math.Cos(2 * de - md) + .007843 * System.Math.Cos(2 * de) +
                  .002824 * System.Math.Cos(2 * md) + .000857 * System.Math.Cos(2 * de + md) + e * .000533 * System.Math.Cos(2 * de - ms) +
                  e * .000401 * System.Math.Cos(2 * de - md - ms) + e * .00032 * System.Math.Cos(md - ms) - .000271 * System.Math.Cos(de) -
                  e * .000264 * System.Math.Cos(ms + md) - .000198 * System.Math.Cos(2 * f - md);
            hp = hp + .000173 * System.Math.Cos(3 * md) + .000167 * System.Math.Cos(4 * de - md) - e * .000111 * System.Math.Cos(ms) +
                 .000103 * System.Math.Cos(4 * de - 2 * md) - .000084 * System.Math.Cos(2 * md - 2 * de) -
                 e * .000083 * System.Math.Cos(2 * de + ms) + .000079 * System.Math.Cos(2 * de + 2 * md) + .000072 * System.Math.Cos(4 * de) +
                 e * .000064 * System.Math.Cos(2 * de - ms + md) - e * .000063 * System.Math.Cos(2 * de + ms - md) +
                 e * .000041 * System.Math.Cos(ms + de);
            hp = hp + e * .000035 * System.Math.Cos(2 * md - ms) - .000033 * System.Math.Cos(3 * md - 2 * de) -
                 .00003 * System.Math.Cos(md + de) - .000029 * System.Math.Cos(2 * (f - de)) - e * .000029 * System.Math.Cos(2 * md + ms) +
                 e2 * .000026 * System.Math.Cos(2 * (de - ms)) - .000023 * System.Math.Cos(2 * (f - de) + md) +
                 e * .000019 * System.Math.Cos(4 * de - ms - md);
            hp = degrad(hp);
        }

        private static double seps, ceps;	/* sin and cos of mean obliquity */

        public static void ecleq_aux(int sw, double mjd, double x, double y, out double p, out double q)
        /* +1 for eq to ecliptic, -1 for vv. */
        /* sw==1: x==ra, y==dec.  sw==-1: x==lng, y==lat. */
        /* sw==1: p==lng, q==lat. sw==-1: p==ra, q==dec. */
        {
            double sx, cx, sy, cy, ty;

            double eps;
            double deps, dpsi;
            obliquity(mjd, out eps);		/* mean obliquity for date */
            nutation(mjd, out deps, out dpsi);
            eps += deps;
            seps = System.Math.Sin(eps);
            ceps = System.Math.Cos(eps);

            sy = System.Math.Sin(y);
            cy = System.Math.Cos(y);				/* always non-negative */
            if (System.Math.Abs(cy) < 1e-20)
                cy = 1e-20;		/* insure > 0 */
            ty = sy / cy;
            cx = System.Math.Cos(x);

            sx = System.Math.Sin(x);
            q = System.Math.Asin((sy * ceps) - (cy * seps * sx * sw));
            p = System.Math.Atan(((sx * ceps) + (ty * seps * sw)) / cx);
            if (cx < 0) p += System.Math.PI;		/* account for atan quad ambiguity */

            range(ref p, 2 * System.Math.PI);
        }


        /* given latitude (n+, radians), lat, hour angle (radians), ha, and declination
         * (radians), dec,
         * return altitude (up+, radians), alt, and
         * azimuth (angle round to the east from north+, radians),
         */
        public static void hadec_aa(double lat, double ha, double dec, out double alt, out double az)
        {
            aaha_aux(lat, ha, dec, out az, out alt);
        }

        public static void aaha_aux(double lat, double x, double y, out double p, out double q)
        {
            double cap, B;

            double slat = System.Math.Sin(lat);
            double clat = System.Math.Cos(lat);

            solve_sphere(-x, System.Math.PI / 2 - y, slat, clat, out cap, out B);

            p = B;
            q = System.Math.PI / 2 - System.Math.Acos(cap);
        }


        /* solve a spherical triangle:
         *           A
         *          /  \
         *         /    \
         *      c /      \ b
         *       /        \
         *      /          \
         *    B ____________ C
         *           a
         *
         * given A, b, c find B and a in range 0..B..2PI, 0..a..PI
         * N.B. we pass in cos(c) and sin(c) because in many problems one of the sides
         *   remains constant for many values of A and b.
         */
        public static void solve_sphere(double A, double b, double cc, double sc, out double cap, out double Bp)
        {
            double cA = System.Math.Cos(A), sA = System.Math.Sin(A);
            double cb = System.Math.Cos(b), sb = System.Math.Sin(b);
            double ca;
            double B;
            double x, y;

            ca = cb * cc + sb * sc * cA;
            if (ca > 1.0) ca = 1.0;
            if (ca < -1.0) ca = -1.0;

            if (cc > .99999)
            {
                /* as c approaches 0, B approaches pi - A */
                B = System.Math.PI - A;
            }
            else if (cc < -.99999)
            {
                /* as c approaches PI, B approaches A */
                B = A;
            }
            else
            {
                /* compute cB and sB and remove common factor of sa from quotient.
                 * be careful where B causes atan to blow.
                 */
                y = sA * sb * sc;
                x = cb - ca * cc;

                if (System.Math.Abs(x) < 1e-5)
                    B = y < 0 ? 3 * System.Math.PI / 2 : System.Math.PI / 2;
                else
                    B = System.Math.Atan2(y, x);
            }

            cap = ca;
            Bp = B;
            range(ref Bp, 2 * System.Math.PI);
        }

        public static double JD_from_Date(DateTime dt)
        {
            int Year = dt.Year;
            int Month = dt.Month;
            double day = dt.Day + (dt.Hour + dt.Minute / 60.0 + dt.Second / 3600.0) / 24.0;

            double num = ((12.0 * (Convert.ToDouble(Year) + 4800.0)) + Month) - 3.0;
            double num2 = Math.Floor((double)((((2.0 * (num % 12.0)) + 7.0) + (365.0 * num)) / 12.0)) + day;
            double num3 = (num2 + Math.Floor((double)(num / 48.0))) - 32083.5;
            if (((Year > 0x62e) | ((Year == 0x62e) & (Month > 10))) | (((Year == 0x62e) & (Month == 10)) & (day > 4.0)))
            {
                num3 = ((num3 + Math.Floor((double)(num / 4800.0))) - Math.Floor((double)(num / 1200.0))) + 38.0;
            }
            return num3;
        }

        public static double SiderealTime_deg(double JD)
        {
            double num2 = Math.Floor(JD) + 0.5;
            double num = (num2 - 2451545.0) / 36525.0;
            double num3 = (100.46061837 + (36000.770053608 * num)) + ((0.00038793 * num) * num);
            num3 += 360.985647 * (JD - num2);
            num3 = num3 % 360.0;
            return num3;
        }

        public static double LongitudeForAltitudeAndLatitude(
            DateTime moment,
            double altitude,
            double latitude,
            double raInHours,
            double decInDeg,
            out double lng2)
        {
            double degToRadian = 57.295779513082323;
            double siderial = SiderealTime_deg(JD_from_Date(moment));
            altitude = altitude / degToRadian;
            latitude = latitude / degToRadian;
            decInDeg = decInDeg / degToRadian;
            double lng1 = degToRadian * Math.Acos((Math.Sin(altitude) - Math.Sin(latitude) * Math.Sin(decInDeg)) / (Math.Cos(latitude) * Math.Cos(decInDeg)));
            lng2 = -1 * lng1;
            lng1 = lng1 + raInHours * 15 - siderial;
            lng2 = lng2 + raInHours * 15 - siderial;
            range(ref lng1, 360);
            range(ref lng2, 360);
            return lng1;
        }

        public static double GMST_2007_0UT_InHours(DateTime momentUT)
        {
            double[] DATA = new double[12] { 6.6188, 8.6558, 10.4957, 12.5327, 14.5040, 16.5410, 18.5123, 20.5493, 22.5863, 0.5576, 2.5946, 4.5659 };

            return DATA[momentUT.Month - 1] + momentUT.Day * 0.06571;
        }

        public static double sdrl_t(double jd)
        {
            double stdeg, t;
            t = ((jd - 2451545.0) / 36525.0);
            stdeg = (6 * 3600 + 41 * 60 + 50.54841 + 8640184.812866 * t + 0.093104 * t * t - 6.2E-6 * t * t * t) / 3600;
            double st = degrad(stdeg);
            range(ref st, 2 * System.Math.PI);

            return st;
        }

        public static double LocalSideralTimeInHours(DateTime momentUT, double longitudeInDegrees)
        {
            double jd = JD(momentUT);
            double UT_SidTime_At0UT = sdrl_t(jd);
            UT_SidTime_At0UT = degrad(GMST_2007_0UT_InHours(momentUT) * 15);
            double deps, dpsi;
            nutation(GetMJD(momentUT), out deps, out dpsi);
            UT_SidTime_At0UT = UT_SidTime_At0UT + deps;
            double LocSidTimeAt0UT = raddeg(UT_SidTime_At0UT) + longitudeInDegrees;
            double SidTime = LocSidTimeAt0UT / 15 + ((double)momentUT.Hour + (double)momentUT.Minute / 60 + (double)momentUT.Second / 3600);

            range(ref SidTime, 24);

            return SidTime;
        }

        public static void AltAzCoords(double RAInDeg, double DECInDeg, double latInDeg, double longInDeg, DateTime momentUT, out double ALTinDeg, out double AZinDeg)
        {
            double locSidTime = LocalSideralTimeInHours(momentUT, longInDeg);
            double HA = (locSidTime - RAInDeg / 15) * 15;
            range(ref HA, 360);

            hadec_aa(degrad(latInDeg), degrad(HA), degrad(DECInDeg), out ALTinDeg, out AZinDeg);
            ALTinDeg = raddeg(ALTinDeg);
            AZinDeg = raddeg(AZinDeg);
        }


        /* given a modified Julian date, mjd, return the mjd of the new
         * and full moons about then, mjdn and mjdf.
         * TODO: exactly which ones does it find? eg:
         *   5/28/1988 yields 5/15 and 5/31
         *   5/29             6/14     6/29
         */
        public static void moonnf(double mjd, out double mjdn, out double mjdf)
        {
            int mo, yr;
            double dy;
            double mjd0;
            double k, tn, tf, t;

            DateTime dt = GetDateFromMJD(mjd);
            mo = dt.Month;
            dy = dt.Day;
            yr = dt.Year;

            mjd0 = GetMJD(new DateTime(yr, 1, 1));

            k = (yr - 1900 + ((mjd - mjd0) / 365)) * 12.3685;
            k = System.Math.Floor(k + 0.5);
            tn = k / 1236.85;
            tf = (k + 0.5) / 1236.85;
            t = tn;
            m(t, k, out mjdn);
            t = tf;
            k += 0.5;
            m(t, k, out mjdf);
        }

        public static void GetMoonPhases(DateTime currentDate, out DateTime newMoon, out DateTime fullMoon)
        {
            double mjdn;
            double mjdf;

            moonnf(GetMJD(currentDate), out mjdn, out mjdf);

            newMoon = GetDateFromMJD(mjdn);
            fullMoon = GetDateFromMJD(mjdf);
        }

        private static double unw(double w, double z)
        {
            return ((w) - System.Math.Floor((w) / (z)) * (z));
        }
        static void m(double t, double k, out double mjd)
        {
            double t2, a, a1, b, b1, c, ms, mm, f, ddjd;

            t2 = t * t;
            a = 29.53 * k;
            c = degrad(166.56 + (132.87 - 9.173e-3 * t) * t);
            b = 5.8868e-4 * k + (1.178e-4 - 1.55e-7 * t) * t2 + 3.3e-4 * System.Math.Sin(c) + 7.5933E-1;
            ms = 359.2242 + 360 * unw(k / 1.236886e1, 1) - (3.33e-5 + 3.47e-6 * t) * t2;
            mm = 306.0253 + 360 * unw(k / 9.330851e-1, 1) + (1.07306e-2 + 1.236e-5 * t) * t2;
            f = 21.2964 + 360 * unw(k / 9.214926e-1, 1) - (1.6528e-3 + 2.39e-6 * t) * t2;
            ms = unw(ms, 360);
            mm = unw(mm, 360);
            f = unw(f, 360);
            ms = degrad(ms);
            mm = degrad(mm);
            f = degrad(f);
            ddjd = (1.734e-1 - 3.93e-4 * t) * System.Math.Sin(ms) + 2.1e-3 * System.Math.Sin(2 * ms)
                - 4.068e-1 * System.Math.Sin(mm) + 1.61e-2 * System.Math.Sin(2 * mm) - 4e-4 * System.Math.Sin(3 * mm)
                + 1.04e-2 * System.Math.Sin(2 * f) - 5.1e-3 * System.Math.Sin(ms + mm) - 7.4e-3 * System.Math.Sin(ms - mm)
                + 4e-4 * System.Math.Sin(2 * f + ms) - 4e-4 * System.Math.Sin(2 * f - ms) - 6e-4 * System.Math.Sin(2 * f + mm)
                + 1e-3 * System.Math.Sin(2 * f - mm) + 5e-4 * System.Math.Sin(ms + 2 * mm);
            a1 = (long)a;
            b = b + ddjd + (a - a1);
            b1 = (long)b;
            a = a1 + b1;
            b = b - b1;
            mjd = a + b;
        }

        /* given the modified JD, mjd, return the true geocentric ecliptic longitude
         *   of the sun for the mean equinox of the date, *lsn, in radians, and the
         *   sun-earth distance, *rsn, in AU. (the true ecliptic latitude is never more
         *   than 1.2 arc seconds and so may be taken to be a constant 0.)
         * if the APPARENT ecliptic longitude is required, correct the longitude for
         *   nutation to the true equinox of date and for aberration (light travel time,
         *   approximately  -9.27e7/186000/(3600*24*365)*2*pi = -9.93e-5 radians).
         */
        public static void sunpos(double mjd, out double lsn, out double rsn)
        {
            double t, t2;
            double ls, ms;    /* mean longitude and mean anomoay */
            double s, nu, ea; /* eccentricity, true anomaly, eccentric anomaly */
            double a, b, a1, b1, c1, d1, e1, h1, dl, dr;

            t = mjd / 36525.0;
            t2 = t * t;
            a = 100.0021359 * t;
            b = 360.0 * (a - (long)a);
            ls = 279.69668 + 0.0003025 * t2 + b;
            a = 99.99736042000039 * t;
            b = 360 * (a - (long)a);
            ms = 358.47583 - (0.00015 + 0.0000033 * t) * t2 + b;
            s = 0.016751 - 0.0000418 * t - 1.26e-07 * t2;

            anomaly(degrad(ms), s, out nu, out ea);

            a = 62.55209472000015 * t;
            b = 360 * (a - (long)a);
            a1 = degrad(153.23 + b);
            a = 125.1041894 * t;
            b = 360 * (a - (long)a);
            b1 = degrad(216.57 + b);
            a = 91.56766028 * t;
            b = 360 * (a - (long)a);
            c1 = degrad(312.69 + b);
            a = 1236.853095 * t;
            b = 360 * (a - (long)a);
            d1 = degrad(350.74 - 0.00144 * t2 + b);
            e1 = degrad(231.19 + 20.2 * t);
            a = 183.1353208 * t;
            b = 360 * (a - (long)a);
            h1 = degrad(353.4 + b);
            dl = 0.00134 * System.Math.Cos(a1) + 0.00154 * System.Math.Cos(b1) + 0.002 * System.Math.Cos(c1) + .00179 * System.Math.Sin(d1) + 0.00178 * System.Math.Sin(e1);
            dr = 5.43e-06 * System.Math.Sin(a1) + 1.575e-05 * System.Math.Sin(b1) + 1.627e-05 * System.Math.Sin(c1) + 3.076e-05 * System.Math.Cos(d1) + 9.27e-06 * System.Math.Sin(h1);

            lsn = nu + degrad(ls - ms + dl);

            while (lsn < 0) { lsn += 2 * System.Math.PI; };
            while (lsn > 2 * System.Math.PI) { lsn -= 2 * System.Math.PI; };

            rsn = 1.0000002 * (1 - s * System.Math.Cos(ea)) + dr;
        }

        /* given the mean anomaly, ma, and the eccentricity, s, of elliptical motion,
         * find the true anomaly, *nu, and the eccentric anomaly, *ea.
         * all angles in radians.
         */
        private static void anomaly(double ma, double s, out double nu, out double ea)
        {
            double m, fea, corr;
            /* double m1 */

            if (s < 1.0)
            {
                /* elliptical */
                double dla;
                m = ma - System.Math.PI * 2 * (long)(ma / (System.Math.PI * 2));
                if (m > System.Math.PI) m -= System.Math.PI * 2;
                if (m < -System.Math.PI) m += System.Math.PI * 2;
                /*  The current method is ok but smoe tempting mods require
                 *	0<M<PI. Substitute m1 for m if so.
                 *	m1 = fabs(m);
                 */
                fea = m;

                for (; ; )
                {
                    dla = fea - (s * System.Math.Sin(fea)) - m;
                    if (System.Math.Abs(dla) < 1e-6)
                        break;
                    /* avoid runnaway corrections for e>.97 and M near 0*/
                    corr = 1 - (s * System.Math.Cos(fea));
                    if (corr < 0.1) corr = 0.1;
                    dla /= corr;
                    fea -= dla;
                }

                nu = 2 * System.Math.Atan(System.Math.Sqrt((1 + s) / (1 - s)) * System.Math.Tan(fea / 2));
            }
            else
            {
                throw new NotSupportedException();
                ///* hyperbolic */
                //double fea1, corr = 1;

                //m = fabs(ma);
                //fea = m / (s-1.);
                //fea1 = pow(6*m/(s*s),1./3.);
                ///* whichever is smaller is the better initial guess */
                //if (fea1 < fea) fea = fea1;

                //while (fabs(corr) > 0.000001) {
                //  corr = (m - s * sinh(fea) + fea) / (s*cosh(fea) - 1);
                //  fea += corr;
                //}
                //if (ma < 0.) fea = -fea;
                //*nu = 2*atan(sqrt((s+1)/(s-1))*tanh(fea/2));
            }

            ea = fea;
        }

        public static double Elongation(double ra1, double de1, double ra2, double de2)
        {
            // http://www.movable-type.co.uk/scripts/latlong.html

            double dLat = (de2 - de1) * System.Math.PI / 180;
            double dLong = (ra2 - ra1) * System.Math.PI / 180;

            double a = System.Math.Sin(dLat / 2) * System.Math.Sin(dLat / 2) +
                    System.Math.Cos(de1 * System.Math.PI / 180) * System.Math.Cos(de2 * System.Math.PI / 180) *
                    System.Math.Sin(dLong / 2) * System.Math.Sin(dLong / 2);

            double c = 2 * System.Math.Atan2(System.Math.Sqrt(a), System.Math.Sqrt(1 - a));

            return c * 180 / Math.PI;
        }
    }
}
