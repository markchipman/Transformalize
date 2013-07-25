﻿/*
Transformalize - Replicate, Transform, and Denormalize Your Data...
Copyright (C) 2013 Dale Newman

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using NUnit.Framework;

namespace Transformalize.Test.Unit {
    [TestFixture]
    public class TestSqlTemplates {

        [Test]
        public void TestTruncateSql() {
            Assert.AreEqual(@"
                IF EXISTS(
        	        SELECT *
        	        FROM INFORMATION_SCHEMA.TABLES
        	        WHERE TABLE_SCHEMA = 'dbo'
        	        AND TABLE_NAME = 'Test'
                )	TRUNCATE TABLE [dbo].[Test];
            ", SqlTemplates.TruncateTable("Test"));
        }

        [Test]
        public void TestDropSql() {
            Assert.AreEqual(@"
                IF EXISTS(
        	        SELECT *
        	        FROM INFORMATION_SCHEMA.TABLES
        	        WHERE TABLE_SCHEMA = 'dbo'
        	        AND TABLE_NAME = 'TEST'
                )	DROP TABLE [dbo].[TEST];
            ", SqlTemplates.DropTable("TEST"));
        }
   
    }
}