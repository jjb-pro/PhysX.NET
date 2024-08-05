﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhysX.Test.Util;

namespace PhysX.Test.Cooking
{
	[TestClass]
	public class CookTest : Test
	{
		/// <summary>
		/// Cook a convex mesh (a teapot).
		/// </summary>
		[TestMethod]
		public void CookConvexMesh()
		{
			string teapotXml;
			using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("PhysX.Test.Resources.Teapot.DAE"))
			{
				var streamReader = new StreamReader(stream);

				teapotXml = streamReader.ReadToEnd();
			}

			using (var physics = CreatePhysicsAndScene())
			{
				var colladaLoader = new ColladaLoader();

				// Read the vertices and indices from the Teapot.DAE collada file
				// This file is copied out to the TestResults folder using the DeploymentItem attribute on this class
				var teapot = colladaLoader.Load(teapotXml);

				using (var cooking = physics.Physics.CreateCooking())
				{
					var desc = new ConvexMeshDesc();
					desc.SetTriangles(teapot.Indices);
					desc.SetPositions(teapot.Vertices);
					desc.Flags = ConvexFlag.ComputeConvex;

					var stream = new MemoryStream();

					var result = cooking.CookConvexMesh(desc, stream);

					Assert.IsFalse(physics.ErrorOutput.HasErrors, physics.ErrorOutput.LastError);
					Assert.AreEqual(ConvexMeshCookingResult.Success, result);
				}
			}
		}
	}
}