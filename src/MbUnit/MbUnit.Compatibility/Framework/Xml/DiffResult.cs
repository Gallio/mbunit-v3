// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

//	Original XmlUnit license
/*
******************************************************************
Copyright (c) 2001, Jeff Martin, Tim Bacon
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions
are met:

    * Redistributions of source code must retain the above copyright
      notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above
      copyright notice, this list of conditions and the following
      disclaimer in the documentation and/or other materials provided
      with the distribution.
    * Neither the name of the xmlunit.sourceforge.net nor the names
      of its contributors may be used to endorse or promote products
      derived from this software without specific prior written
      permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING,
BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN
ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
POSSIBILITY OF SUCH DAMAGE.

******************************************************************
*/

#pragma warning disable 1591
#pragma warning disable 3001

namespace MbUnit.Framework.Xml 
{
	using System; 
	using System.Text; 
	
    public class DiffResult {
        private bool _identical = true;
        private bool _equal = true;
        private Difference _difference;
    	private StringBuilder _stringBuilder;
    	
    	public DiffResult() {
    		_stringBuilder = new StringBuilder();
    	}
        
        public bool Identical {
            get {
                return _identical;
            }
        }
        
        public bool Equal {
            get {
                return _equal;
            }
        }
        
        public Difference Difference {
            get {
                return _difference;
            }
        }
     
        public string StringValue {
        	get {
	        	if (_stringBuilder.Length == 0) {
	        		if (Identical) {
	        			_stringBuilder.Append("Identical");        			
	        		} else {
	        			_stringBuilder.Append("Equal");
	        		}
	        	}
	        	return _stringBuilder.ToString();
        	}
        }
        
        public void DifferenceFound(XmlDiff inDiff, Difference difference) {
            _identical = false;
            if (difference.MajorDifference) {
                _equal = false;
            }       
            _difference = difference;
        	if (_stringBuilder.Length == 0) {
        		_stringBuilder.Append(inDiff.OptionalDescription);
        	}
        	_stringBuilder.Append(Environment.NewLine).Append(difference);
        }        
    }
	
}
