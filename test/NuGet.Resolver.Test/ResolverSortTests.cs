﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using NuGet.Resolver;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using NuGet.Packaging;
using System.Threading;

namespace NuGet.Resolver.Test
{
    public class ResolverSortTests
    {
        [Fact]
        public void ResolverSort_Ignore()
        {
            HashSet<PackageIdentity> installed = new HashSet<PackageIdentity>();
            HashSet<string> newPackages = new HashSet<string>();

            var comparer = new ResolverComparer(DependencyBehavior.Ignore, installed, newPackages);

            var packages = new List<ResolverPackage>(VersionList.Select(e => new ResolverPackage("packageA", e)));

            packages.Sort(comparer);

            Assert.Equal("3.0.9", packages.First().Version.ToNormalizedString());
            Assert.Equal("0.1.0", packages.Last().Version.ToNormalizedString());
        }

        [Fact]
        public void ResolverSort_IgnoreInstalled()
        {
            HashSet<PackageIdentity> installed = new HashSet<PackageIdentity>();
            installed.Add(new PackageIdentity("packageA", NuGetVersion.Parse("2.0.0")));

            HashSet<string> newPackages = new HashSet<string>();

            var comparer = new ResolverComparer(DependencyBehavior.Ignore, installed, newPackages);

            var packages = new List<ResolverPackage>(VersionList.Select(e => new ResolverPackage("packageA", e)));

            packages.Sort(comparer);

            // Ignore should still use the installed package
            Assert.Equal("2.0.0", packages.First().Version.ToNormalizedString());
            Assert.Equal("0.1.0", packages.Last().Version.ToNormalizedString());
        }

        [Fact]
        public void ResolverSort_HighestMinor()
        {
            HashSet<PackageIdentity> installed = new HashSet<PackageIdentity>();
            HashSet<string> newPackages = new HashSet<string>();

            var comparer = new ResolverComparer(DependencyBehavior.HighestMinor, installed, newPackages);

            var packages = new List<ResolverPackage>(VersionList.Select(e => new ResolverPackage("packageA", e)));

            packages.Sort(comparer);

            Assert.Equal("0.3.1", packages.First().Version.ToNormalizedString());
            Assert.Equal("3.0.0", packages.Last().Version.ToNormalizedString());
        }

        [Fact]
        public void ResolverSort_HighestMinorInstalled()
        {
            HashSet<PackageIdentity> installed = new HashSet<PackageIdentity>();
            installed.Add(new PackageIdentity("packageA", NuGetVersion.Parse("2.0.0")));

            HashSet<string> newPackages = new HashSet<string>();

            var comparer = new ResolverComparer(DependencyBehavior.HighestMinor, installed, newPackages);

            var packages = new List<ResolverPackage>(VersionList.Select(e => new ResolverPackage("packageA", e)));

            packages.Sort(comparer);

            // take the installed one first
            Assert.Equal("2.0.0", packages.First().Version.ToNormalizedString());
        }

        [Fact]
        public void ResolverSort_HighestMinorPreferUpgrade()
        {
            HashSet<PackageIdentity> installed = new HashSet<PackageIdentity>();
            installed.Add(new PackageIdentity("packageA", NuGetVersion.Parse("2.0.0")));

            HashSet<string> newPackages = new HashSet<string>();

            var comparer = new ResolverComparer(DependencyBehavior.HighestMinor, installed, newPackages);

            VersionRange removeRange = new VersionRange(NuGetVersion.Parse("2.0.0"), true, NuGetVersion.Parse("2.0.0"), true);

            var packages = new List<ResolverPackage>(VersionList.Where(e => !removeRange.Satisfies(e)).Select(e => new ResolverPackage("packageA", e)));

            packages.Sort(comparer);

            // take the upgrade of the highest minor
            Assert.Equal("2.5.0", packages.First().Version.ToNormalizedString());
        }

        [Fact]
        public void ResolverSort_HighestMinorPreferHighestDowngrade()
        {
            HashSet<PackageIdentity> installed = new HashSet<PackageIdentity>();
            installed.Add(new PackageIdentity("packageA", NuGetVersion.Parse("2.0.0")));

            HashSet<string> newPackages = new HashSet<string>();

            var comparer = new ResolverComparer(DependencyBehavior.HighestMinor, installed, newPackages);

            VersionRange removeRange = new VersionRange(NuGetVersion.Parse("2.0.0"), true);

            var packages = new List<ResolverPackage>(VersionList.Where(e => !removeRange.Satisfies(e)).Select(e => new ResolverPackage("packageA", e)));

            packages.Sort(comparer);

            // take the highest downgrade
            Assert.Equal("1.3.2", packages.First().Version.ToNormalizedString());
        }


        [Fact]
        public void ResolverSort_HighestPatch()
        {
            HashSet<PackageIdentity> installed = new HashSet<PackageIdentity>();
            HashSet<string> newPackages = new HashSet<string>();

            var comparer = new ResolverComparer(DependencyBehavior.HighestPatch, installed, newPackages);

            var packages = new List<ResolverPackage>(VersionList.Select(e => new ResolverPackage("packageA", e)));

            packages.Sort(comparer);

            Assert.Equal("0.1.2", packages.First().Version.ToNormalizedString());
            Assert.Equal("3.0.0", packages.Last().Version.ToNormalizedString());
        }

        [Fact]
        public void ResolverSort_HighestPatchInstalled()
        {
            HashSet<PackageIdentity> installed = new HashSet<PackageIdentity>();
            installed.Add(new PackageIdentity("packageA", NuGetVersion.Parse("2.0.0")));

            HashSet<string> newPackages = new HashSet<string>();

            var comparer = new ResolverComparer(DependencyBehavior.HighestPatch, installed, newPackages);

            var packages = new List<ResolverPackage>(VersionList.Select(e => new ResolverPackage("packageA", e)));

            packages.Sort(comparer);

            // take the installed one first
            Assert.Equal("2.0.0", packages.First().Version.ToNormalizedString());
        }

        [Fact]
        public void ResolverSort_HighestPatchPreferUpgrade()
        {
            HashSet<PackageIdentity> installed = new HashSet<PackageIdentity>();
            installed.Add(new PackageIdentity("packageA", NuGetVersion.Parse("2.0.0")));

            HashSet<string> newPackages = new HashSet<string>();

            var comparer = new ResolverComparer(DependencyBehavior.HighestPatch, installed, newPackages);

            VersionRange removeRange = new VersionRange(NuGetVersion.Parse("2.0.0"), true, NuGetVersion.Parse("2.0.0"), true);

            var packages = new List<ResolverPackage>(VersionList.Where(e => !removeRange.Satisfies(e)).Select(e => new ResolverPackage("packageA", e)));

            packages.Sort(comparer);

            // take the upgrade of the highest patch
            Assert.Equal("2.0.2", packages.First().Version.ToNormalizedString());
        }

        [Fact]
        public void ResolverSort_HighestPatchPreferHighestDowngrade()
        {
            HashSet<PackageIdentity> installed = new HashSet<PackageIdentity>();
            installed.Add(new PackageIdentity("packageA", NuGetVersion.Parse("2.0.0")));

            HashSet<string> newPackages = new HashSet<string>();

            var comparer = new ResolverComparer(DependencyBehavior.HighestPatch, installed, newPackages);

            VersionRange removeRange = new VersionRange(NuGetVersion.Parse("2.0.0"), true);

            var packages = new List<ResolverPackage>(VersionList.Where(e => !removeRange.Satisfies(e)).Select(e => new ResolverPackage("packageA", e)));

            packages.Sort(comparer);

            // take the highest downgrade
            Assert.Equal("1.3.2", packages.First().Version.ToNormalizedString());
        }



        [Fact]
        public void ResolverSort_Highest()
        {
            HashSet<PackageIdentity> installed = new HashSet<PackageIdentity>();
            HashSet<string> newPackages = new HashSet<string>();

            var comparer = new ResolverComparer(DependencyBehavior.Highest, installed, newPackages);

            var packages = new List<ResolverPackage>(VersionList.Select(e => new ResolverPackage("packageA", e)));

            packages.Sort(comparer);

            Assert.Equal("3.0.9", packages.First().Version.ToNormalizedString());
            Assert.Equal("0.1.0", packages.Last().Version.ToNormalizedString());
        }

        [Fact]
        public void ResolverSort_HighestInstalled()
        {
            HashSet<PackageIdentity> installed = new HashSet<PackageIdentity>();
            installed.Add(new PackageIdentity("packageA", NuGetVersion.Parse("2.0.0")));

            HashSet<string> newPackages = new HashSet<string>();

            var comparer = new ResolverComparer(DependencyBehavior.Highest, installed, newPackages);

            var packages = new List<ResolverPackage>(VersionList.Select(e => new ResolverPackage("packageA", e)));

            packages.Sort(comparer);

            // highest should still use the installed package
            Assert.Equal("2.0.0", packages.First().Version.ToNormalizedString());
            Assert.Equal("0.1.0", packages.Last().Version.ToNormalizedString());
        }

        [Fact]
        public void ResolverSort_HighestUpgrade()
        {
            HashSet<PackageIdentity> installed = new HashSet<PackageIdentity>();
            installed.Add(new PackageIdentity("packageA", NuGetVersion.Parse("2.0.0")));

            HashSet<string> newPackages = new HashSet<string>();

            var comparer = new ResolverComparer(DependencyBehavior.Highest, installed, newPackages);

            VersionRange removeRange = new VersionRange(NuGetVersion.Parse("2.0.0"), true, NuGetVersion.Parse("2.0.1"), true);

            var packages = new List<ResolverPackage>(VersionList.Where(e => !removeRange.Satisfies(e)).Select(e => new ResolverPackage("packageA", e)));

            packages.Sort(comparer);

            // ignore the installed package if it is not in allowed
            Assert.Equal("3.0.9", packages.First().Version.ToNormalizedString());
            Assert.Equal("0.1.0", packages.Last().Version.ToNormalizedString());
        }


        [Fact]
        public void ResolverSort_Lowest()
        {
            HashSet<PackageIdentity> installed = new HashSet<PackageIdentity>();
            HashSet<string> newPackages = new HashSet<string>();

            var comparer = new ResolverComparer(DependencyBehavior.Lowest, installed, newPackages);

            var packages = new List<ResolverPackage>(VersionList.Select(e => new ResolverPackage("packageA", e)));

            packages.Sort(comparer);

            Assert.Equal("0.1.0", packages.First().Version.ToNormalizedString());
        }

        [Fact]
        public void ResolverSort_LowestInstalled()
        {
            HashSet<PackageIdentity> installed = new HashSet<PackageIdentity>();
            installed.Add(new PackageIdentity("packageA", NuGetVersion.Parse("2.0.0")));

            HashSet<string> newPackages = new HashSet<string>();

            var comparer = new ResolverComparer(DependencyBehavior.Lowest, installed, newPackages);

            var packages = new List<ResolverPackage>(VersionList.Select(e => new ResolverPackage("packageA", e)));

            packages.Sort(comparer);

            // take the installed one first
            Assert.Equal("2.0.0", packages.First().Version.ToNormalizedString());
        }

        [Fact]
        public void ResolverSort_LowestPreferUpgrade()
        {
            HashSet<PackageIdentity> installed = new HashSet<PackageIdentity>();
            installed.Add(new PackageIdentity("packageA", NuGetVersion.Parse("2.0.0")));

            HashSet<string> newPackages = new HashSet<string>();

            var comparer = new ResolverComparer(DependencyBehavior.Lowest, installed, newPackages);

            VersionRange removeRange = new VersionRange(NuGetVersion.Parse("2.0.0"), true, NuGetVersion.Parse("2.0.1"), true);

            var packages = new List<ResolverPackage>(VersionList.Where(e => !removeRange.Satisfies(e)).Select(e => new ResolverPackage("packageA", e)));

            packages.Sort(comparer);

            // take the upgrade first
            Assert.Equal("2.0.2", packages.First().Version.ToNormalizedString());
        }

        [Fact]
        public void ResolverSort_LowestPreferHighestDowngrade()
        {
            HashSet<PackageIdentity> installed = new HashSet<PackageIdentity>();
            installed.Add(new PackageIdentity("packageA", NuGetVersion.Parse("2.0.0")));

            HashSet<string> newPackages = new HashSet<string>();

            var comparer = new ResolverComparer(DependencyBehavior.Lowest, installed, newPackages);

            VersionRange removeRange = new VersionRange(NuGetVersion.Parse("2.0.0"), true);

            var packages = new List<ResolverPackage>(VersionList.Where(e => !removeRange.Satisfies(e)).Select(e => new ResolverPackage("packageA", e)));

            packages.Sort(comparer);

            // take the highest downgrade
            Assert.Equal("1.3.2", packages.First().Version.ToNormalizedString());
        }


        private static List<NuGetVersion> VersionList = new List<NuGetVersion>()
        {
            NuGetVersion.Parse("0.1.0"),
            NuGetVersion.Parse("0.1.1"),
            NuGetVersion.Parse("0.1.2"),
            NuGetVersion.Parse("0.2.0"),
            NuGetVersion.Parse("0.3.0"),
            NuGetVersion.Parse("0.3.1"),
            NuGetVersion.Parse("1.0.0"),
            NuGetVersion.Parse("1.0.1"),
            NuGetVersion.Parse("1.1.1"),
            NuGetVersion.Parse("1.1.2"),
            NuGetVersion.Parse("1.1.3"),
            NuGetVersion.Parse("1.2.0"),
            NuGetVersion.Parse("1.3.1"),
            NuGetVersion.Parse("1.3.2"),
            NuGetVersion.Parse("2.0.0"),
            NuGetVersion.Parse("2.0.1"),
            NuGetVersion.Parse("2.0.2"),
            NuGetVersion.Parse("2.5.0"),
            NuGetVersion.Parse("3.0.0"),
            NuGetVersion.Parse("3.0.9"),
        };
    }
}