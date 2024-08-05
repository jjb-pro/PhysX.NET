﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhysX.Test
{
	[TestClass]
	public class HeightFieldTest : Test
	{
		[TestMethod]
		public void CreateAndDisposeHeightField()
		{
			using (var physics = CreatePhysicsAndScene())
			{
				const int rows = 25, columns = 25;
				var samples = HeightFieldTestUtil.CreateSampleGrid(rows, columns);

				var heightFieldDesc = new HeightFieldDesc
				{
					NumberOfRows = rows,
					NumberOfColumns = columns,
					Samples = samples
				};

				var cooking = physics.Physics.CreateCooking();

				var stream = new MemoryStream();
				bool cookResult = cooking.CookHeightField(heightFieldDesc, stream);

				stream.Position = 0;

				HeightField heightField;
				using (heightField = physics.Physics.CreateHeightField(stream))
				{
					Assert.IsNotNull(heightField);
					Assert.IsFalse(heightField.Disposed);
				}

				Assert.IsTrue(heightField.Disposed);
			}
		}

		[TestMethod]
		public void MultipleDisposeCallsDoNotCauseAnException()
		{
			using (var physics = CreatePhysicsAndScene())
			{
				const int rows = 25, columns = 25;
				var samples = HeightFieldTestUtil.CreateSampleGrid(rows, columns);

				var heightFieldDesc = new HeightFieldDesc()
				{
					NumberOfRows = rows,
					NumberOfColumns = columns,
					Samples = samples
				};

				var cooking = physics.Physics.CreateCooking();

				var stream = new MemoryStream();
				bool cookResult = cooking.CookHeightField(heightFieldDesc, stream);

				stream.Position = 0;

				HeightField heightField = physics.Physics.CreateHeightField(stream);

				Assert.IsFalse(heightField.Disposed);

				// Dispose
				heightField.Dispose();
				Assert.IsTrue(heightField.Disposed);

				// Dispose again
				heightField.Dispose();
				Assert.IsTrue(heightField.Disposed);
			}
		}

		[TestMethod]
		public void MultipleInstancesAreValid()
		{
			using (var physics = CreatePhysicsAndScene())
			{
				// A
				{
					const int rows = 25, columns = 25;
					var samples = HeightFieldTestUtil.CreateSampleGrid(rows, columns);

					var heightFieldDesc = new HeightFieldDesc()
					{
						NumberOfRows = rows,
						NumberOfColumns = columns,
						Samples = samples
					};

					var cooking = physics.Physics.CreateCooking();

					var stream = new MemoryStream();
					bool cookResult = cooking.CookHeightField(heightFieldDesc, stream);

					stream.Position = 0;

					HeightField heightField = physics.Physics.CreateHeightField(stream);
				}

				// B
				{
					const int rows = 25, columns = 25;
					var samples = HeightFieldTestUtil.CreateSampleGrid(rows, columns);

					var heightFieldDesc = new HeightFieldDesc()
					{
						NumberOfRows = rows,
						NumberOfColumns = columns,
						Samples = samples
					};

					var cooking = physics.Physics.CreateCooking();

					var stream = new MemoryStream();
					bool cookResult = cooking.CookHeightField(heightFieldDesc, stream);

					stream.Position = 0;

					HeightField heightField = physics.Physics.CreateHeightField(stream);
				}
			}
		}
	}
}