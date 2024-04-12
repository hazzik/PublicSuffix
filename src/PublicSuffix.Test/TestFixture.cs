﻿using System;
using System.Diagnostics;
using System.Globalization;
using NUnit.Framework;

namespace Brandy.PublicSuffix.Test
{
    [TestFixture]
    public class TestFixture
    {
        private readonly DomainParser _parser;

        public TestFixture()
        {
            var sw = new Stopwatch();
            sw.Start();
            _parser = DomainParser.FromFile("effective_tld_names.dat");
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }

        [TestCaseSource(typeof (TestCaseSource), nameof(TestCaseSource.Source)), Test]
        public void Test(string host, string expected)
        {
            Console.WriteLine(host);
            Console.WriteLine(expected);
            var domain = Parse(host);
            if (domain == null)
            {
                Assert.AreEqual(null, expected);
            }
            else
            {
                Assert.AreEqual(expected, domain.RegistrableDomain);
            }
        }

        private Domain Parse(string host)
        {
            try
            {
                var sw = new Stopwatch();
                sw.Start();
                var domain = _parser.Parse(host);
                sw.Stop();
                Console.WriteLine(sw.ElapsedMilliseconds);
                return domain;
            }
            catch (Exception)
            {
                return null;
            }
        }

        [Test]
        public void Performance()
        {
            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < 1000000; i++)
            {
                var domain = _parser.Parse("xxx.www.takahama.aichi.jp");
                Assert.AreEqual("www.takahama.aichi.jp", domain.RegistrableDomain);
            }
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }
    }
}