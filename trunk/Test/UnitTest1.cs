﻿using System;
using System.Reflection;
using Cs2hx;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Test;

namespace UnitTestProject1
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void OfType()
		{
			TestFramework.TestCode(MethodInfo.GetCurrentMethod().Name, @"
using System.Text;
using System.Linq;

namespace Blargh
{
    public class SomeClass
    {
        public SomeClass()
        {
            var a = new[] { 1,2,3 };
			var b = a.OfType<StringBuilder>().ToList();
        }
    }
}", @"
package blargh;
" + WriteImports.StandardImports + @"
import system.linq.Linq;
import system.text.StringBuilder;

class SomeClass
{
    public function new()
    {
        var a:Array<Int> = [ 1, 2, 3 ];
		var b:Array<StringBuilder> = Linq.ToList(Linq.OfType(a, StringBuilder));
    }
}");
		}

		[TestMethod]
		public void GlobalKeyword()
		{
			TestFramework.TestCode(MethodInfo.GetCurrentMethod().Name, @"
using System;

namespace Blargh
{
    public static class SomeClass
    {
        public SomeClass()
        {
            global::Blargh.SomeClass c = null;
        }
    }
}", @"
package blargh;
" + WriteImports.StandardImports + @"

class SomeClass
{
    public function new()
    {
        var c:Blargh.SomeClass = null;
    }
}");
		}


		[TestMethod]
		public void DefaultParameter()
		{

			TestFramework.TestCode(MethodInfo.GetCurrentMethod().Name, @"
using System;

namespace Blargh
{
    public class SomeClass
    {
        public void Foo(int i1, int i2 = 4, string s1 = ""hi"")
        {
        }

        public SomeClass(int i3 = 9)
        {
            Foo(4);
            Foo(5, 6);
            Foo(6, 7, ""eight"");
        }
    }
}", @"
package blargh;
" + WriteImports.StandardImports + @"

class SomeClass
{
    public function Foo(i1:Int, i2:Int = 4, s1:String = ""hi""):Void
    {
    }

    public function new(i3:Int = 9)
    {
        Foo(4);
        Foo(5, 6);
        Foo(6, 7, ""eight"");
    }
}");
		}

		[TestMethod]
		public void ForStatementWithNoCondition()
		{

			TestFramework.TestCode(MethodInfo.GetCurrentMethod().Name, @"
using System;

namespace Blargh
{
    public static class Utilities
    {
        public static void SomeFunction()
        {
            for(;;)
            {
                Console.WriteLine(""Hello, World!"");
            }
        }
    }
}", @"
package blargh;
" + WriteImports.StandardImports + @"

class Utilities
{
    public static function SomeFunction():Void
    {
        { //for
            while (true)
            {
                Console.WriteLine(""Hello, World!"");
            }
        } //end for
    }
    public function new()
    {
    }
}");
		}

		[TestMethod]
		public void AutomaticProperties()
		{
			TestFramework.TestCode(MethodInfo.GetCurrentMethod().Name, @"
using System;

namespace Blargh
{
    class Box
    {
        public float Width
        {
            get;
            set;
        }
    }
}", @"
package blargh;
" + WriteImports.StandardImports + @"

class Box
{
    public var Width:Float;
    public function new()
    {
    }
}");
		}

		[TestMethod]
		public void GenericClass()
		{
			TestFramework.TestCode(MethodInfo.GetCurrentMethod().Name, @"
using System;
using System.Collections.Generic;

namespace Blargh
{
	public class KeyValueList<K, V> : IEquatable<K>
	{
		private List<KeyValuePair<K, V>> _list = new List<KeyValuePair<K, V>>();

		public void Add(K key, V value)
		{
			this._list.Add(new KeyValuePair<K, V>(key, value));
		}

		public void Insert(int index, K key, V value)
		{
			_list.Insert(index, new KeyValuePair<K, V>(key, value));
		}

		public void Clear()
		{
			_list.Clear();
			var castTest = (K)MemberwiseClone();
		}

		public void RemoveAt(int index)
		{
			_list.RemoveAt(index);
		}

		public bool Equals(K other)
		{
			throw new NotImplementedException();
		}
	}
}", @"
package blargh;
" + WriteImports.StandardImports + @"
import system.collections.generic.KeyValuePair;
import system.NotImplementedException;

class KeyValueList<K, V> implements IEquatable<K>
{
    private var _list:Array<KeyValuePair<K, V>>;

    public function Add(key:K, value:V):Void
    {
        this._list.push(new KeyValuePair<K, V>(key, value));
    }
    public function Insert(index:Int, key:K, value:V):Void
    {
        _list.insert(index, new KeyValuePair<K, V>(key, value));
    }
    public function Clear():Void
    {
        _list.splice(0, _list.length);
        var castTest:K = cast(MemberwiseClone(), K);
    }
    public function RemoveAt(index:Int):Void
    {
        _list.splice(index, 1);
    }
    public function new()
    {
        _list = new Array<KeyValuePair<K, V>>();
    }
	public function Equals(other:K):Bool
	{
		throw new NotImplementedException();
	}
}");
		}

		[TestMethod]
		public void ByteArrays()
		{
			TestFramework.TestCode(MethodInfo.GetCurrentMethod().Name, @"
using System;

namespace Blargh
{
    public static class Utilities
    {
        public static void SomeFunction()
        {
            byte[] b1 = new byte[4];
			byte[] b2 = new byte[Foo()];
        }
		static int Foo() { return 4; }
    }
}", @"
package blargh;
" + WriteImports.StandardImports + @"
import haxe.io.Bytes;

class Utilities
{
    public static function SomeFunction():Void
    {
        var b1:Bytes = Bytes.alloc(4);
		var b2:Bytes = Bytes.alloc(Foo());
    }
	static function Foo():Int
	{
		return 4;
	}
    public function new()
    {
    }
}");
		}

		[TestMethod]
		[ExpectedException(typeof(Exception), "Cannot use \"continue\" in a \"for\" loop")]
		public void CannotUseContinueInForLoop()
		{
			TestFramework.TestCode(MethodInfo.GetCurrentMethod().Name, @"
using System;

namespace Blargh
{
    public static class SomeClass
    {
        public void SomeMethod()
        {
            for(int i=0;i<40;i++)
            {
                if (i % 3 == 0)
                    continue;
                Console.WriteLine(i);
            }
        }
    }
}", "");
		}

		[TestMethod]
		public void ConstructorCallsBaseConstructor()
		{
			var cs = @"
using System;

namespace Blargh
{
    public class Top
    {
        public Top(int i) { }
    }

    public class Derived : Top
    {
        public Derived() : base(4) { }
    }
}";

			var haxe1 = @" 
package blargh;
" + WriteImports.StandardImports + @"

class Top
{
    public function new(i:Int)
    {
    }
}";

			var haxe2 = @"
package blargh;
" + WriteImports.StandardImports + @"

class Derived extends Top
{
    public function new()
    {
        super(4);
    }
}";

			TestFramework.TestCode(MethodInfo.GetCurrentMethod().Name, cs, new string[] { haxe1, haxe2 });
		}

		[TestMethod]
		public void ImportStatements()
		{
			var cSharp = @"
namespace SomeClassNamespace
{
    using SomeInterfaceNamespace;

    public class SomeClass : ISomeInterface
    {
        public void SomeClassMethod() { }
    }
}

namespace SomeInterfaceNamespace
{
    public interface ISomeInterface
    {
    }

    public class UnusedClass
    {
    }
}";

			var haxe1 = @"
package someclassnamespace;
" + WriteImports.StandardImports + @"
import someinterfacenamespace.ISomeInterface;

class SomeClass implements ISomeInterface
{
    public function SomeClassMethod():Void
    {
    }
    
    public function new()
    {
    }
}";

			var haxe2 = @"
package someinterfacenamespace;
" + WriteImports.StandardImports + @"

interface ISomeInterface
{
}";
			var haxe3 = @"
package someinterfacenamespace;
" + WriteImports.StandardImports + @"

class UnusedClass
{
    public function new()
    {
    }
}";

			TestFramework.TestCode(MethodInfo.GetCurrentMethod().Name, cSharp, new string[] { haxe1, haxe2, haxe3 });
		}

		[TestMethod]
		public void TypeInference()
		{
			TestFramework.TestCode(MethodInfo.GetCurrentMethod().Name, @"
using System;
using System.Text;

namespace Blargh
{
    public class Box
    {
        public static void Main()
        {
            SomeFunction((_, o) => o + 1);
        }

        public static int SomeFunction(Func<StringBuilder, int, int> doWork)
        {
            var value = doWork(null, 3);
			return value;
        }

        public Box(Action<DateTime> doWork)
        {
        }
    }
}", @"
package blargh;
" + WriteImports.StandardImports + @"
import system.DateTime;
import system.text.StringBuilder;

class Box
{
    public static function Main():Void
    {
        SomeFunction(function (_:StringBuilder, o:Int):Int { return o + 1; } );
    }

    public static function SomeFunction(doWork:(StringBuilder -> Int -> Int)):Int
    {
        var value:Int = doWork(null, 3);
		return value;
    }
    public function new(doWork:(DateTime -> Void))
    {
    }
}
");
		}

		[TestMethod]
		[ExpectedException(typeof(Exception), "C# 3.5 object initialization syntax is not supported")]
		public void ObjectInitilization()
		{
			TestFramework.TestCode("test", @"
namespace Blargh
{
    public static class Utilities
    {
        public static void SomeFunction()
        {
            var usingMe = new SomeUsingType()
            {
                Init1 = 0
            };
        }
    }
}", "");
		}

		[TestMethod]
		[ExpectedException(typeof(Exception), "You cannot return from within a using block")]
		public void CannotReturnFromUsing()
		{
			TestFramework.TestCode(MethodInfo.GetCurrentMethod().Name, @"
namespace Blargh
{
    public static class Utilities
    {
        public static int SomeFunction()
        {
            var usingMe = new SomeUsingType();
            using (usingMe)
            {
                return 4;
            }
        }
    }
}", "");
		}

		[TestMethod]
		public void UsingStatement()
		{

			TestFramework.TestCode(MethodInfo.GetCurrentMethod().Name, @"
namespace Blargh
{
    public static class Utilities
    {
        public static void SomeFunction()
        {
            var usingMe = new SomeUsingType();
            using (usingMe)
            {
                Console.WriteLine(""In using"");
            }
        }
    }
}", @"
package blargh;
" + WriteImports.StandardImports + @"

class Utilities
{
    public static function SomeFunction():Void
    {
        var usingMe:SomeUsingType = new SomeUsingType();
        var __disposed_usingMe:Bool = false;
        try
        {
            Console.WriteLine(""In using"");

            __disposed_usingMe = true;
            usingMe.Dispose();
        }
        catch (__catch_usingMe:Dynamic)
        {
            if (!__disposed_usingMe)
                usingMe.Dispose();
            throw __catch_usingMe;
        }
    }
    public function new()
    {
    }
}
");
		}
		[TestMethod]
		public void Math()
		{
			TestFramework.TestCode(MethodInfo.GetCurrentMethod().Name, @"
using System;

namespace Blargh
{
    public static class Utilities
    {
        public static void SomeFunction()
        {
            int i = 3;
            i += 4;
            i -= 3;
            ++i;
            i++;
            i--;
            --i;
            i *= 4;
            i %= 3;
            i = i + 1;
            i = i % 3;
            i = i - 4;
            i = i * 100;
            double f = i / 3f;
            int hex = 0x00ff;
            i = (int)f;
        }
    }
}", @"
package blargh;
" + WriteImports.StandardImports + @"

class Utilities
{
    public static function SomeFunction():Void
    {
        var i:Int = 3;
        i += 4;
        i -= 3;
        ++i;
        i++;
        i--;
        --i;
        i *= 4;
        i %= 3;
        i = i + 1;
        i = i % 3;
        i = i - 4;
        i = i * 100;
        var f:Float = i / 3;
        var hex:Int = 0x00ff;
        i = Std.int(f);
    }
    public function new()
    {
    }
}");
		}

		[TestMethod]
		public void Delegates()
		{
			TestFramework.TestCode(MethodInfo.GetCurrentMethod().Name, @"
using System;

namespace Blargh
{
    public delegate int NamespaceDlg();
    public delegate T TemplatedDelegate<T>(T arg, int arg2);

    public static class Utilities
    {
        public delegate int GetMahNumber(int arg);

        public static void SomeFunction(GetMahNumber getit, NamespaceDlg getitnow, TemplatedDelegate<float> unused)
        {
            Console.WriteLine(getit(getitnow()));
            
        }
    }
}", @"
package blargh;
" + WriteImports.StandardImports + @"

class Utilities
{
    public static function SomeFunction(getit:(Int -> Int), getitnow:(Void -> Int), unused:(Float -> Int -> Float)):Void
    {
        Console.WriteLine(getit(getitnow()));
    }
    public function new()
    {
    }
}");
		}

		[TestMethod]
		public void TypeStatics()
		{
			TestFramework.TestCode(MethodInfo.GetCurrentMethod().Name, @"
using System;

namespace Blargh
{
    public static class Utilities
    {
        public static void SomeFunction()
        {
            StringBuilder.DateTime.MythicalField = 4;
            Console.WriteLine(int.MaxValue);
            Console.WriteLine(int.MinValue);
            string s = ""123"";
            Console.WriteLine(int.Parse(s) + 1);
            float.Parse(s);
            double.Parse(s);
        }
    }
}", @"
package blargh;
" + WriteImports.StandardImports + @"
import system.text.StringBuilder;

class Utilities
{
    public static function SomeFunction():Void
    {
        StringBuilder.DateTime.MythicalField = 4;
        Console.WriteLine(2147483647);
        Console.WriteLine(-2147483648);
        var s:String = ""123"";
        Console.WriteLine(Std.parseInt(s) + 1);
        Std.parseFloat(s);
        Std.parseFloat(s);
    }
    public function new()
    {
    }
}");
		}

		[TestMethod]
		public void DictionaryAndHashSet()
		{
			TestFramework.TestCode(MethodInfo.GetCurrentMethod().Name, @"
using System;
using System.Collections.Generic;

namespace Blargh
{
    public static class Utilities
    {
        public static void SomeFunction()
        {
            Dictionary<int, int> dict = new Dictionary<int, int>();
            dict.Add(4, 3);
            Console.WriteLine(dict[4]);
            Console.WriteLine(dict.ContainsKey(8));
            dict.Remove(4);
            foreach(int key in dict.Keys)
                Console.WriteLine(key);
            foreach(int val in dict.Values)
                Console.WriteLine(val);
            
            HashSet<int> hash = new HashSet<int>();
            hash.Add(999);
            Console.WriteLine(hash.Contains(999));
            hash.Remove(999);
            Console.WriteLine(hash.Contains(999));
            foreach(int hashItem in hash)
                Console.WriteLine(hashItem);
        }
    }
}", @"
package blargh;
" + WriteImports.StandardImports + @"
import system.collections.generic.CSDictionary;
import system.collections.generic.HashSet;

class Utilities
{
    public static function SomeFunction():Void
    {
        var dict:CSDictionary<Int, Int> = new CSDictionary<Int, Int>();
        dict.Add(4, 3);
        Console.WriteLine(dict.GetValue(4));
        Console.WriteLine(dict.ContainsKey(8));
        dict.Remove(4);
        for (key in dict.Keys)
        {
            Console.WriteLine(key);
        }
        for (val in dict.Values)
        {
            Console.WriteLine(val);
        }
        
        var hash:HashSet<Int> = new HashSet<Int>();
        hash.Add(999);
        Console.WriteLine(hash.Contains(999));
        hash.Remove(999);
        Console.WriteLine(hash.Contains(999));
        for (hashItem in hash.Values())
        {
            Console.WriteLine(hashItem);
        }
    }
    public function new()
    {
    }
}");
		}

		[TestMethod]
		public void NullableTypes()
		{

			TestFramework.TestCode(MethodInfo.GetCurrentMethod().Name, @"
using System;

namespace Blargh
{
    public static class Utilities
    {
        public static void SomeFunction()
        {
            int? nullableInt = new Nullable<int>();
            Console.WriteLine(nullableInt.HasValue);
            int? withValue = new Nullable<int>(8);
            Console.WriteLine(withValue.Value);
        }
    }
}", @"
package blargh;
" + WriteImports.StandardImports + @"
import system.Nullable_Int;

class Utilities
{
    public static function SomeFunction():Void
    {
        var nullableInt:Nullable_Int = new Nullable_Int();
        Console.WriteLine(nullableInt.HasValue);
        var withValue:Nullable_Int = new Nullable_Int(8);
        Console.WriteLine(withValue.Value);
    }
    public function new()
    {
    }
}");
		}

		[TestMethod]
		public void Enums()
		{

			TestFramework.TestCode(MethodInfo.GetCurrentMethod().Name, new string[] { @"
namespace Blargh
{
    public enum MostlyNumbered
    {
        One = 1,
        Two = 2,
        Three = 3,
        Unnumbered,
        SomethingElse = 50
    }
    public enum UnNumbered
    {
        One, Two, Three
    }
    class Clazz
    {
        public static void Methodz()
        {
            var f = MostlyNumbered.One;
            var arr = new UnNumbered[] { UnNumbered.One, UnNumbered.Two, UnNumbered.Three };
            var i = (int)f;
        }
    }
}" }, new string[] { @"
package blargh;
" + WriteImports.StandardImports + @"
class MostlyNumbered
{
    public static inline var One:Int = 1;
    public static inline var Two:Int = 2;
    public static inline var Three:Int = 3;
    public static inline var Unnumbered:Int = 4;
    public static inline var SomethingElse:Int = 50;
}", @"
package blargh;
" + WriteImports.StandardImports + @"
class UnNumbered
{
	public static inline var One:Int = 1; 
    public static inline var Two:Int = 2;
    public static inline var Three:Int = 3;
}", @"
package blargh;
" + WriteImports.StandardImports + @"
class Clazz
{
    public static function Methodz():Void
    {
        var f:Int = MostlyNumbered.One;
        var arr:Array<Int> = [ UnNumbered.One, UnNumbered.Two, UnNumbered.Three ];
        var i:Int = f;
    }
    public function new()
    {
    }
}"});
		}
		[TestMethod]
		public void SwitchStatement()
		{

			TestFramework.TestCode(MethodInfo.GetCurrentMethod().Name, @"
using System;

namespace Blargh
{
    public static class Utilities
    {
        public static void SomeFunction()
        {
            string s = ""Blah"";
            switch (s)
            {
                case ""NotMe"": Console.WriteLine(4); break;
                case ""Box"": Console.WriteLine(4); break;
                case ""Blah"": Console.WriteLine(3); break;
                default: throw new InvalidOperationException();
            }
        }
    }
}", @"
package blargh;
" + WriteImports.StandardImports + @"
import system.InvalidOperationException;

class Utilities
{
    public static function SomeFunction():Void
    {
        var s:String = ""Blah"";
        switch (s)
        {
            case ""NotMe"":
                Console.WriteLine(4);
            case ""Box"": 
                Console.WriteLine(4); 
            case ""Blah"": 
                Console.WriteLine(3); 
            default: 
                throw new InvalidOperationException();
        }
    }
    public function new()
    {
    }
}");
		}
		[TestMethod]
		public void Linq()
		{

			TestFramework.TestCode(MethodInfo.GetCurrentMethod().Name, @"
using System;
using System.Linq;

namespace Blargh
{
    public static class Utilities
    {
        public static void SomeFunction()
        {
            int[] e = new int[] { 0, 1, 2, 3 };
            Console.WriteLine(e.First());
            Console.WriteLine(e.First(o => o == 1));
            Console.WriteLine(e.ElementAt(2));
            Console.WriteLine(e.Last());
            Console.WriteLine(e.Count());
            Console.WriteLine(e.Where(o => o > 0).Count() + 2);
            Console.WriteLine(e.Count(o => true) + 2);

            var dict = e.ToDictionary(o => o, o => 555);
            e.OfType<int>();
        }
    }
}", @"
package blargh;
" + WriteImports.StandardImports + @"
import system.collections.generic.CSDictionary;
import system.linq.Linq;

class Utilities
{
    public static function SomeFunction():Void
    {
        var e:Array<Int> = [ 0, 1, 2, 3 ];
        Console.WriteLine(Linq.First(e));
		
        Console.WriteLine(Linq.First(e, function (o:Int):Bool
        {
            return o == 1;
        } ));
        Console.WriteLine(Linq.ElementAt(e, 2));
        Console.WriteLine(Linq.Last(e));
        Console.WriteLine(Linq.Count(e));
        Console.WriteLine(Linq.Count(Linq.Where(e, function (o:Int):Bool
        {
            return o > 0;
        } )) + 2);
        Console.WriteLine(Linq.Count(e, function (o:Int):Bool
        {
            return true;
        } ) + 2);
        var dict:CSDictionary<Int, Int> = Linq.ToDictionary(e, function (o:Int):Int
        {
            return o;
        } , function (o:Int):Int
        {
            return 555;
        } );
        Linq.OfType(e, Int);
    }
    public function new()
    {
    }
}");
		}

		[TestMethod]
		public void OverloadedMethods()
		{
			TestFramework.TestCode(MethodInfo.GetCurrentMethod().Name, @"
using System;

namespace Blargh
{
    public static class Utilities
    {
        public static void OverOne()
        {
            OverOne(3, ""Blah"");
        }

        public static void OverOne(int param)
        {
            OverOne(3, ""Blah"");
        }

        public static void OverOne(int param, string prm)
        {
            Console.WriteLine(param + prm);
        }

        public static int OverTwo(int prm)
        {
            return prm;
        }
        public static int OverTwo()
        {
            return OverTwo(18);
        }
    }
}", @"
package blargh;
" + WriteImports.StandardImports + @"

class Utilities
{
    public static function OverOne(param:Int = 3, prm:String = ""Blah""):Void
    {
        Console.WriteLine(param + prm);
    }
    public static function OverTwo(prm:Int = 18):Int
    {
        return prm;
    }
    public function new()
    {
    }
}");
		}


		[TestMethod]
		public void IsAndAs()
		{

			TestFramework.TestCode(MethodInfo.GetCurrentMethod().Name, @"
using System;

namespace Blargh
{
    public static class Utilities
    {
        public static void SomeFunction()
        {
            string s = ""Blah"";
            var list = new List<int>();
            if (s is string)
                Console.WriteLine(""Yes"");
            if (list is List<int>)
                Console.WriteLine(""Yes"");

//            object o = s;
//            string sss = o as string;
//            Console.WriteLine(sss);
        }
    }
}", @"
package blargh;
" + WriteImports.StandardImports + @"

class Utilities
{
    public static function SomeFunction():Void
    {
        var s:String = ""Blah"";
        var list:Array<Int> = new Array<Int>();
        if (Std.is(s, String))
        {
            Console.WriteLine(""Yes"");
        }
        if (Std.is(list, Array))
        {
            Console.WriteLine(""Yes"");
        }
    }
    public function new()
    {
    }
}");
		}


		[TestMethod]
		public void AbstractAndOverrides()
		{
			TestFramework.TestCode(MethodInfo.GetCurrentMethod().Name, @"
using System;

namespace Blargh
{
    abstract class TopLevel
    {
        public abstract void AbstractMethod();
        public abstract string AbstractProperty { get; }

        public virtual void VirtualMethod()
        {
            Console.WriteLine(""TopLevel::VirtualMethod"");
        }
        public virtual string VirtualProperty
        {
            get
            {
                return ""TopLevel::VirtualProperty"";
            }
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }

    class Derived : TopLevel
    {
        public override void AbstractMethod()
        {
            Console.WriteLine(""Derived::AbstractMethod"");
        }

        public override string AbstractProperty
        {
            get { return ""Derived::AbstractProperty""; }
        }

        public override void VirtualMethod()
        {
            base.VirtualMethod();
            Console.WriteLine(""Derived::VirtualMethod"");
        }

        public override string VirtualProperty
        {
            get
            {
                return base.VirtualProperty + ""Derived:VirtualProperty"";
            }
        }
        public override string ToString()
        {
            return ""DerivedToString"";
        }
    }
}", new string[] {
      @"
package blargh;
" + WriteImports.StandardImports + @"
class TopLevel
{
	public var AbstractProperty(get_AbstractProperty, never):String;
	
    public function get_AbstractProperty():String
    {
    	throw new Exception(""Abstract item called"");
		return null;
    }

    public var VirtualProperty(get_VirtualProperty, never):String;
    public function get_VirtualProperty():String
    {
        return ""TopLevel::VirtualProperty"";
    }

    public function AbstractMethod():Void
    {
    	throw new Exception(""Abstract item called"");
    }
    public function VirtualMethod():Void
    {
        Console.WriteLine(""TopLevel::VirtualMethod"");
    }
    public function toString():String
    {
        return """";
    }
	
	public function new()
	{
	}
}",
    @"
package blargh;
" + WriteImports.StandardImports + @"
class Derived extends TopLevel
{
    override public function get_AbstractProperty():String
    {
        return ""Derived::AbstractProperty"";
    }
    override public function get_VirtualProperty():String
    {
        return super.VirtualProperty + ""Derived:VirtualProperty"";
    }

    override public function AbstractMethod():Void
    {
        Console.WriteLine(""Derived::AbstractMethod"");
    }
    override public function VirtualMethod():Void
    {
        super.VirtualMethod();
        Console.WriteLine(""Derived::VirtualMethod"");
    }
    override public function toString():String
    {
        return ""DerivedToString"";
    }
	
	public function new()
	{
		super();
	}
}"  });
		}


		[TestMethod]
		public void FieldsAndProperties()
		{
			TestFramework.TestCode(MethodInfo.GetCurrentMethod().Name, @"
using System;
using System.Text;

namespace Blargh
{
    class Box
    {
        private float _width;
        public float Width
        {
            get { return _width; }
            set { _width = value; }
        }

        public float SetOnly
        {
            set { Console.WriteLine(value); }
        }

        public int GetOnly
        {
            get { return 4; }
        }
        
        public bool IsRectangular = true;
        public char[] Characters = new char[] { 'a', 'b' };
        public static StringBuilder StaticField = new StringBuilder();
        public const int ConstInt = 24;
        public static readonly int StaticReadonlyInt = 5;
        public const string WithQuoteMiddle = @""before""""after"";
        public const string WithQuoteStart = @""""""after"";
        public int MultipleOne, MultipleTwo;
        public readonly int ReadonlyInt = 3;

        static Box()
        {
            Console.WriteLine(""cctor"");
        }

        public Box()
        {
            Console.WriteLine(""ctor"");
        }
    }
}", @"
package blargh;
" + WriteImports.StandardImports + @"
import system.text.StringBuilder;

class Box
{
    private var _width:Float;
    public var IsRectangular:Bool;
    public var Characters:Array<Int>;
    public static var StaticField:StringBuilder;
    public static inline var ConstInt:Int = 24;
    public static inline var StaticReadonlyInt:Int = 5;
    public static inline var WithQuoteMiddle:String = ""before\""after"";
    public static inline var WithQuoteStart:String = ""\""after"";
    public var MultipleOne:Int;
    public var MultipleTwo:Int;
    public var ReadonlyInt:Int;

    public var Width(get_Width, set_Width):Float;
    public function get_Width():Float
    {
        return _width;
    }
    public function set_Width(value:Float):Float
    {
        _width = value;
		return 0;
    }
    public var SetOnly(never, set_SetOnly):Float;
    public function set_SetOnly(value:Float):Float
    {
        Console.WriteLine(value);
		return 0;
    }
    public var GetOnly(get_GetOnly, never):Int;
    public function get_GetOnly():Int
    {
        return 4;
    }

    public static function cctor():Void
    {
        StaticField = new StringBuilder();
        Console.WriteLine(""cctor"");
    }

	public function new()
	{
		IsRectangular = true;
		Characters = [ 97, 98 ];
        ReadonlyInt = 3;
        Console.WriteLine(""ctor"");
	}
}");
		}



		[TestMethod]
		public void Interfaces()
		{
			TestFramework.TestCode(MethodInfo.GetCurrentMethod().Name, @"
using System;

namespace Blargh
{
    public interface ITesting
    {
        void Poke();
    }

    class Pokable : ITesting
    {
        public void Poke()
        {
            Console.WriteLine(""Implementation"");
        }
    }
}",
  new string[] { @"
package blargh;
" + WriteImports.StandardImports + @"

interface ITesting
{
    function Poke():Void;
}",
  @"
package blargh;
" + WriteImports.StandardImports + @"

class Pokable implements ITesting
{
    public function Poke():Void
    {
        Console.WriteLine(""Implementation"");
    }
    public function new()
    {
    }
}"});
		}

		[TestMethod]
		public void TryCatchThrow()
		{
			TestFramework.TestCode(MethodInfo.GetCurrentMethod().Name, @"
using System;

namespace Blargh
{
    public static class Utilities
    {
        public static void SomeFunction()
        {
            Console.WriteLine(""Before try"");
            try
            {
                Console.WriteLine(""In try"");
            }
            catch (Exception ex)
            {
                Console.WriteLine(""In catch"");
            }

            try
            {
                Console.WriteLine(""Try without finally"");
            }
            catch (IOException ex)
            {
                Console.WriteLine(""In second catch"");
            }

            try
            {
                Console.WriteLine(""Try in parameterless catch"");
            }
            catch
            {
                Console.WriteLine(""In parameterless catch"");
            }

            throw new InvalidOperationException(StringBuilder.MythicalField);
        }
    }
}", @"
package blargh;
" + WriteImports.StandardImports + @"
import system.Exception;
import system.InvalidOperationException;
import system.text.StringBuilder;

class Utilities
{
    public static function SomeFunction():Void
    {
        Console.WriteLine(""Before try"");
        try
        {
            Console.WriteLine(""In try"");
        }
        catch (ex:Exception)
        {
            Console.WriteLine(""In catch"");
        }
        try
        {
            Console.WriteLine(""Try without finally"");
        }
        catch (ex:IOException)
        {
            Console.WriteLine(""In second catch"");
        }
        try
        {
            Console.WriteLine(""Try in parameterless catch"");
        }
        catch (__ex:Dynamic)
        {
            Console.WriteLine(""In parameterless catch"");
        }

        throw new InvalidOperationException(StringBuilder.MythicalField);
    }
    public function new()
    {
    }
}");
		}



		[TestMethod]
		public void Generics()
		{
			TestFramework.TestCode(MethodInfo.GetCurrentMethod().Name, @"
using System;
using System.Collections.Generic;

namespace Blargh
{
    public static class Utilities
    {
        public static Queue<T> ToQueue<T>(this IEnumerable<T> array)
        {
            var queue = new Queue<T>();
            foreach (T a in array)
                queue.Enqueue(a);

            queue.Dequeue();
            return queue;
        }

        public static IEnumerable<T> SideEffect<T>(this IEnumerable<T> array, Action<T> effect)
        {
            foreach(var i in array)
                effect(i);
            return array;
        }
    }
}", @"
package blargh;
" + WriteImports.StandardImports + @"

class Utilities
{
    public static function ToQueue<T>(array:Array<T>):Array<T>
    {
        var queue:Array<T> = new Array<T>();
        for (a in array)
        {
            queue.push(a);
        }
        queue.shift();
        return queue;
    }
    public static function SideEffect<T>(array:Array<T>, effect:(T -> Void)):Array<T>
    {
        for (i in array)
        {
            effect(i);
        }
        return array;
    }
    public function new()
    {
    }
}");
		}

		[TestMethod]
		public void Objects()
		{
			TestFramework.TestCode(MethodInfo.GetCurrentMethod().Name, @"
using System;
using System.Collections.Generic;

namespace Blargh
{
    public static class Utilities
    {
        public static void SomeFunction()
        {
            var queue = new Queue<int>(10);
            queue.Enqueue(4);
            queue.Enqueue(2);
            Console.WriteLine(queue.Dequeue());
            queue.Clear();
    
            var list = new List<string>(3);
            list.Add(""Three"");
            list.RemoveAt(0);
            list.Insert(4, ""Seven"");

            var stack = new Stack<int>();
            stack.Push(9);
            stack.Push(3);
            Math.Max(stack.Pop(), stack.Pop());
        }
    }
}", @"
package blargh;
" + WriteImports.StandardImports + @"

class Utilities
{
    public static function SomeFunction():Void
    {
        var queue:Array<Int> = new Array<Int>();
        queue.push(4);
        queue.push(2);
        Console.WriteLine(queue.shift());
        queue.splice(0, queue.length);

        var list:Array<String> = new Array<String>();
        list.push(""Three"");
        list.splice(0, 1);
        list.insert(4, ""Seven"");

        var stack:Array<Int> = new Array<Int>();
        stack.push(9);
        stack.push(3);
        Math.max(stack.pop(), stack.pop());
    }
    public function new()
    {
    }
}");
		}

		[TestMethod]
		public void Lambda()
		{
			TestFramework.TestCode(MethodInfo.GetCurrentMethod().Name, @"
using System;
using System.Collections.Generic;

namespace Blargh
{
    public static class Utilities
    {
        public static void SomeFunction()
        {
            Func<int, int> f1 = x => x + 5;
            Console.WriteLine(f1(3));
            Func<int, int> f2 = x => { return x + 6; };
            Console.WriteLine(f2(3));

            List<Action> actions = new List<Action>();
        }
    }
}", @"
package blargh;
" + WriteImports.StandardImports + @"

class Utilities
{
    public static function SomeFunction():Void
    {
        var f1:(Int -> Int) = function (x:Int):Int 
        { 
            return x + 5; 
        } ;
        Console.WriteLine(f1(3));
        var f2:(Int -> Int) = function (x:Int):Int 
        { 
            return x + 6; 
        } ;
        Console.WriteLine(f2(3));
        var actions:Array<(Void -> Void)> = new Array<(Void -> Void)>();
    }
    public function new()
    {
    }
}");
		}

		[TestMethod]
		public void LambdaNoReturn()
		{
			TestFramework.TestCode(MethodInfo.GetCurrentMethod().Name, @"
using System;

namespace Blargh
{
    public static class Utilities
    {
        public static void SomeFunction()
        {
            int i = 3;
            Action a = () => i = 4;
            Func<int> b = () => i = 5;
            Foo(() => i = 6);
        }
        public static void Foo(Action a)
        {
        }
    }
}", @"
package blargh;
" + WriteImports.StandardImports + @"

class Utilities
{
    public static function SomeFunction():Void
    {
        var i:Int = 3;
        var a:(Void -> Void) = function ():Void
        { 
            i = 4;
        } ;
        var b:(Void -> Int) = function ():Int
        { 
            return i = 5;
        } ;
        Foo(function ():Void
        {
            i = 6;
        } );
    }
    public static function Foo(a:(Void -> Void)):Void
    {
    }
    public function new()
    {
    }
}");
		}
		[TestMethod]
		public void Loops()
		{
			TestFramework.TestCode(MethodInfo.GetCurrentMethod().Name, @"
using System;

namespace Blargh
{
    public static class Utilities
    {
        public static void SomeFunction()
        {
            while (true)
            {
                Console.WriteLine(""hi"");
                break;
            }
			
			while (true)
				Console.WriteLine(""nobreak"");

            for (int i=0;i<50;i++)
                Console.WriteLine(i);

            do
            {
                Console.WriteLine(""Dowhile"");
            }
            while (false);
        }
    }
}", @"
package blargh;
" + WriteImports.StandardImports + @"

class Utilities
{
    public static function SomeFunction():Void
    {
        while (true)
        {
            Console.WriteLine(""hi"");
            break;
        }

		while (true)
			Console.WriteLine(""nobreak"");

        { //for
            var i:Int = 0;
            while (i < 50)
            {
                Console.WriteLine(i);
                i++;
            }
        } //end for
        do
        {
            Console.WriteLine(""Dowhile"");
        }
        while (false);
    }
    public function new()
    {
    }
}");
		}

		[TestMethod]
		public void ReplaceTypeWithAttribute()
		{
			TestFramework.TestCode(MethodInfo.GetCurrentMethod().Name, @"
using System;

namespace Blargh
{
    public class Foo
    {
        [Cs2Hx(ReplaceWithType = ""bar.Baz"")]
        public object Obj;
    }
}", @"
package blargh;
" + WriteImports.StandardImports + @"

class Foo
{
    public var Obj:bar.Baz;
    public function new()
    {
    }
}");
		}

		[TestMethod]
		public void CastsWithAs()
		{
			var cs = @"
using System;

namespace Blargh
{
#if !CS2HX
	public static class Utilities
	{
		public static T As<T>(this object o)
		{
			return (T)o;
		}
	}
#endif

    public static class Test
    {
        public static void SomeFunction()
        {
			var z = DateTime.Now.As<String>();
        }
    }
}";
  
			var haxe = @"
package blargh;
" + WriteImports.StandardImports + @"
import system.DateTime;

class Test
{
    public static function SomeFunction():Void
    {
        var z:String = cast(DateTime.Now, String);
    }
    public function new()
    {
    }
}";

			var transform = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Translations>
  <Method SourceObject=""*"" Match=""As"">
    <ReplaceWith>
      <String>cast(</String>
      <Expression />
      <String>, {genericType})</String>
    </ReplaceWith>
  </Method>
</Translations>";

			TestFramework.TestCode(MethodInfo.GetCurrentMethod().Name, new[] { cs }, new[] { haxe }, transform);
		}


		[TestMethod]
		public void ArrayAndForEach()
		{
			TestFramework.TestCode(MethodInfo.GetCurrentMethod().Name, @"
using System;
using System.Collections.Generic;

namespace Blargh
{
    public static class Utilities
    {
        public static void SomeFunction()
        {
            var ar = new int[] { 1, 2, 3 };

            foreach(var i in ar)
                Console.WriteLine(i);

            Console.WriteLine(ar[1]);
            Console.WriteLine(ar.Length);
			Console.WriteLine(new List<string>().Count);
        }
    }
}", @"
package blargh;
" + WriteImports.StandardImports + @"

class Utilities
{
    public static function SomeFunction():Void
    {
        var ar:Array<Int> = [ 1, 2, 3 ];
        for (i in ar)
        {
        	Console.WriteLine(i);
        }
        Console.WriteLine(ar[1]);
        Console.WriteLine(ar.length);
		Console.WriteLine(new Array<String>().length);
    }
    public function new()
    {
    }
}");
		}

		[TestMethod]
		public void PartialClasses()
		{
			TestFramework.TestCode(MethodInfo.GetCurrentMethod().Name,

new string[] { @"
using System;

namespace Blargh
{
    public partial class Utilities
    {
        public void FunFromOne()
        {
            Console.WriteLine(""I'm in one!"");
        }
    }
}",
            
  @"
using System;

namespace Blargh
{
    public partial class Utilities
    {
        public void FunFromTwo()
        {
            Console.WriteLine(""I'm in Two!"");
        }
    }
}"
}, @"
package blargh;
" + WriteImports.StandardImports + @"

class Utilities
{
    public function FunFromOne():Void
    {
        Console.WriteLine(""I'm in one!"");
    }
    public function FunFromTwo():Void
    {
        Console.WriteLine(""I'm in Two!"");
    }
    public function new()
    {
    }
}");
		}

		[TestMethod]
		public void StringMethods()
		{

			TestFramework.TestCode(MethodInfo.GetCurrentMethod().Name, @"
using System;

namespace Blargh
{
    public static class Utilities
    {
        public static void SomeFunction(string s2)
        {
            string s = @""50\0"";
            Console.WriteLine(s.IndexOf(""0""));
            Console.WriteLine(s2.IndexOf(""0""));

            foreach(string s3 in new string[] { ""Hello"" })
                s3.Substring(4, 5);

            int i = 4;
            string si = i.ToString();
			if (si.StartsWith(""asdf""))
				Console.WriteLine(4);
        }
    }
}", @"
package blargh;
" + WriteImports.StandardImports + @"
class Utilities
{
    public static function SomeFunction(s2:String):Void
    {
        var s:String = ""50\\0"";
        Console.WriteLine(s.indexOf(""0""));
        Console.WriteLine(s2.indexOf(""0""));

        for (s3 in [ ""Hello"" ])
        {
            s3.substr(4, 5);
        }
        var i:Int = 4;
        var si:String = Std.string(i);
		if (Cs2Hx.StartsWith(si, ""asdf""))
		{
			Console.WriteLine(4);
		}
    }
    public function new()
    {
    }
}");
		}

		[TestMethod]
		public void ExtensionMethod()
		{

			TestFramework.TestCode(MethodInfo.GetCurrentMethod().Name, @"
using System;

namespace Blargh
{
    public static class Utilities
    {
        public static void SomeFunction()
        {
            int i = -3;
            Console.WriteLine(""false "" + i.IsFour());
            i++;
            i += 6;
            var b = i.IsFour();
            Console.WriteLine(""true "" + b);
            Utilities.IsFour(5);
        }

        public static bool IsFour(this int i)
        {
            return i == 4;
        }
    }
}", @"
package blargh;
" + WriteImports.StandardImports + @"

class Utilities
{
    public static function SomeFunction():Void
    {
        var i:Int = -3;
        Console.WriteLine(""false "" + Utilities.IsFour(i));
        i++;
        i += 6;
        var b:Bool = Utilities.IsFour(i);
        Console.WriteLine(""true "" + b);
        Utilities.IsFour(5);
    }

    public static function IsFour(i:Int):Bool
    {
        return i == 4;
    }
    public function new()
    {
    }
}");
		}

		[TestMethod]
		public void StringJoin()
		{

			TestFramework.TestCode(MethodInfo.GetCurrentMethod().Name, @"
using System;

namespace Blargh
{
    public class Foo
    {
        public Foo()
        {
            var s = string.Join(asdf, "";"");
        }
    }
}", @"
package blargh;
" + WriteImports.StandardImports + @"

class Foo
{
    public function new()
    {
        var s = Cs2Hx.Join(asdf, "";"");
    }
}");
		}

		[TestMethod]
		public void HelloWorld()
		{

			TestFramework.TestCode(MethodInfo.GetCurrentMethod().Name, @"
using System;

namespace Blargh
{
    public static class Utilities
    {
        public static void SomeFunction()
        {
            Console.WriteLine(""Hello, World!"");
        }
    }
}", @"
package blargh;
" + WriteImports.StandardImports + @"

class Utilities
{
    public static function SomeFunction():Void
    {
        Console.WriteLine(""Hello, World!"");
    }
    public function new()
    {
    }
}");
		}
		[TestMethod]
		public void IfStatement()
		{

			TestFramework.TestCode(MethodInfo.GetCurrentMethod().Name, @"
using System;

namespace Blargh
{
    public static class Utilities
    {
        public static void SomeFunction()
        {
            string notInitialized;
            int myNum = 0;
            notInitialized = ""InitMe!"";

            if (myNum > 4)
                myNum = 2;
            else if (notInitialized == ""asdf"")
                myNum = 1;
            else
                myNum = 999;

            Console.WriteLine(myNum == 999 ? ""One"" : ""Two"");
        }
    }
}", @"
package blargh;
" + WriteImports.StandardImports + @"
class Utilities
{
    public static function SomeFunction():Void
    {
        var notInitialized:String;
        var myNum:Int = 0;
        notInitialized = ""InitMe!"";

        if (myNum > 4)
        {
            myNum = 2;
        }
        else if (notInitialized == ""asdf"")
        {
            myNum = 1;
        }
        else
        {
            myNum = 999;
        }

        Console.WriteLine(myNum == 999 ? ""One"" : ""Two"");
    }
    public function new()
    {
    }
}");
		}
	}
}
