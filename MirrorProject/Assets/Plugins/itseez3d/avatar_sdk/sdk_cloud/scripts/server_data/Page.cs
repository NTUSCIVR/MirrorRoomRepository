﻿/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@itseez3D.com>, April 2017
*/

using System;

namespace ItSeez3D.AvatarSdk.Cloud
{
	[Serializable]
	public class Page<DataType>
	{
		/// <summary>
		/// Array of object on the current page.
		/// </summary>
		public DataType[] content;

		#region Page navigation urls

		[NonSerialized]
		public string firstPageUrl;
		[NonSerialized]
		public string prevPageUrl;
		[NonSerialized]
		public string nextPageUrl;
		[NonSerialized]
		public string lastPageUrl;

		#endregion
	}
}

