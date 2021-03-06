﻿using System;
using static Bridge.QUnit.QUnit;

namespace Bridge.React.Tests
{
	public sealed class PropInstanceComparisonTests
	{
		public static PropInstanceComparisonTests Instance { get; } = new PropInstanceComparisonTests();
		private PropInstanceComparisonTests() { }

		public void RunTests()
		{
			Module("PureComponent props comparisons (for ShouldComponentUpdate implementation)");

			RegularBridgeClasses();
			PlainObjectLiterals();
			ConstructorEnabledObjectLiterals();
		}

		private void RegularBridgeClasses()
		{
			Test("Two instances of single-string-property class where string values match", assert =>
			{
				assert.StrictEqual(
					ComponentPropsHelpers.DoPropsReferencesMatch(
						new SingleStringPropertyClass("abc"),
						new SingleStringPropertyClass("abc")
					),
					true
				);
			});

			Test("Two instances of single-function-property class where functions are equivalent anonymous static lambdas that Bridge lifts to named methods", assert =>
			{
				assert.StrictEqual(
					ComponentPropsHelpers.DoPropsReferencesMatch(
						new SingleFunctionPropertyClass(value => Console.WriteLine(value)),
						new SingleFunctionPropertyClass(value => Console.WriteLine(value))
					),
					true
				);
			});

			Test("Two instances of single-function-property class where functions are equivalent bound functions", assert =>
			{
				assert.StrictEqual(
					ComponentPropsHelpers.DoPropsReferencesMatch(
						new SingleFunctionPropertyClass(InstanceMethod),
						new SingleFunctionPropertyClass(InstanceMethod)
					),
					true
				);
			});

			// These two props instances will be given then exact same reference, so this is a really easy case
			Test("Two instances of single-function-property class where functions are the same function property on a known reference", assert =>
			{
				var bindToTarget = new SingleFunctionPropertyClass(value => Console.WriteLine(value));
				assert.StrictEqual(
					ComponentPropsHelpers.DoPropsReferencesMatch(
						new SingleFunctionPropertyClass(bindToTarget.OnChange),
						new SingleFunctionPropertyClass(bindToTarget.OnChange)
					),
					true
				);
			});

			// These C# lamdbas are clearly equivalent but it's not possible for the JavaScript code to know this for sure because it can only see the function content
			// of them and can't tell for sure that they haven't been bound to different instances using JavaScript's bind facility. If the solution is 100% Bridge then
			// this shouldn't be the same case because Bridge will bind functions (using its own mechanism) when it has to. However, if the solution ISN'T entirely from
			// Bridge-compiled C# code then there could be other code creating bound functions and then it would not be reliable to compare the two functions by their
			// content. Currently, there is a switch for this behaviour that is set to false and within an internal class and so it can't be set to true by consumers
			// of Bridge.React (apart from this project, which has access to internal-accessibility members). 
			Test("Two instances of single-function-property class where functions are equivalent anonymous static lambdas that Bridge can't lift to named methods (EXPERIMENTAL)", assert =>
			{
				var outerValue = "abc"; // Having the lambdas access this value prevents them from being lifted into their own methods
				var optimiseFunctionComparisonsBasedOnSolutionBeingPureBridge = ComponentPropsHelpers.OptimiseFunctionComparisonsBasedOnSolutionBeingPureBridge;
				try
				{
					ComponentPropsHelpers.OptimiseFunctionComparisonsBasedOnSolutionBeingPureBridge = true;
					assert.StrictEqual(
						ComponentPropsHelpers.DoPropsReferencesMatch(
							new SingleFunctionPropertyClass(value => Console.WriteLine(outerValue + ":" + value)),
							new SingleFunctionPropertyClass(value => Console.WriteLine(outerValue + ":" + value))
						),
						true
					);
				}
				finally
				{
					ComponentPropsHelpers.OptimiseFunctionComparisonsBasedOnSolutionBeingPureBridge = optimiseFunctionComparisonsBasedOnSolutionBeingPureBridge;
				}
			});

			Test("Two instance of a single-property class where property has custom Equals implementation (where both values are considered equivalent)", assert =>
			{
				assert.StrictEqual(
					ComponentPropsHelpers.DoPropsReferencesMatch(
						new SingleCaseInsensitiveStringPropertyClass(new CaseInsensitiveString("abc")),
						new SingleCaseInsensitiveStringPropertyClass(new CaseInsensitiveString("ABC"))
					),
					true
				);
			});

			// TODO: Check with nulls

			// TODO: Add false-result comparisons
		}

		private void PlainObjectLiterals()
		{
			Test("Two instances of [ObjectLiteral] single-string-property class where string values match", assert =>
			{
				assert.StrictEqual(
					ComponentPropsHelpers.DoPropsReferencesMatch(
						new ObjectLiteralSingleStringPropertyClass { Name = "abc" },
						new ObjectLiteralSingleStringPropertyClass { Name = "abc" }
					),
					true
				);
			});

			Test("Two instances of [ObjectLiteral] single-function-property class where functions are equivalent anonymous static lambdas that Bridge lifts to named methods", assert =>
			{
				assert.StrictEqual(
					ComponentPropsHelpers.DoPropsReferencesMatch(
						new ObjectLiteralSingleFunctionPropertyClass { OnChange = value => Console.WriteLine(value) },
						new ObjectLiteralSingleFunctionPropertyClass { OnChange = value => Console.WriteLine(value) }
					),
					true
				);
			});

			Test("Two instances of [ObjectLiteral] single-function-property class where functions are equivalent bound functions", assert =>
			{
				assert.StrictEqual(
					ComponentPropsHelpers.DoPropsReferencesMatch(
						new ObjectLiteralSingleFunctionPropertyClass { OnChange = InstanceMethod },
						new ObjectLiteralSingleFunctionPropertyClass { OnChange = InstanceMethod }
					),
					true
				);
			});

			// These two props instances will be given then exact same reference, so this is a really easy case
			Test("Two instances of [ObjectLiteral] single-function-property class where functions are the same function property on a known reference", assert =>
			{
				var bindToTarget = new SingleFunctionPropertyClass(value => Console.WriteLine(value));
				assert.StrictEqual(
					ComponentPropsHelpers.DoPropsReferencesMatch(
						new ObjectLiteralSingleFunctionPropertyClass { OnChange = bindToTarget.OnChange },
						new ObjectLiteralSingleFunctionPropertyClass { OnChange = bindToTarget.OnChange }
					),
					true
				);
			});
		}

		private void ConstructorEnabledObjectLiterals()
		{
			Test("Two instances of constructor-supporting [ObjectLiteral] single-string-property class where string values match", assert =>
			{
				assert.StrictEqual(
					ComponentPropsHelpers.DoPropsReferencesMatch(
						new ConstructorObjectLiteralSingleStringPropertyClass("abc"),
						new ConstructorObjectLiteralSingleStringPropertyClass("abc")
					),
					true
				);
			});

			Test("Two instances of constructor-supporting [ObjectLiteral] single-function-property class where functions are equivalent anonymous static lambdas that Bridge lifts to named methods", assert =>
			{
				assert.StrictEqual(
					ComponentPropsHelpers.DoPropsReferencesMatch(
						new ConstructorObjectLiteralSingleFunctionPropertyClass(value => Console.WriteLine(value)),
						new ConstructorObjectLiteralSingleFunctionPropertyClass(value => Console.WriteLine(value))
					),
					true
				);
			});

			Test("Two instances of constructor-supporting [ObjectLiteral] single-function-property class where functions are equivalent bound functions", assert =>
			{
				assert.StrictEqual(
					ComponentPropsHelpers.DoPropsReferencesMatch(
						new ConstructorObjectLiteralSingleFunctionPropertyClass(InstanceMethod),
						new ConstructorObjectLiteralSingleFunctionPropertyClass(InstanceMethod)
					),
					true
				);
			});

			// These two props instances will be given then exact same reference, so this is a really easy case
			Test("Two instances of constructor-supporting [ObjectLiteral] single-function-property class where functions are the same function property on a known reference", assert =>
			{
				var bindToTarget = new SingleFunctionPropertyClass(value => Console.WriteLine(value));
				assert.StrictEqual(
					ComponentPropsHelpers.DoPropsReferencesMatch(
						new ConstructorObjectLiteralSingleFunctionPropertyClass(bindToTarget.OnChange),
						new ConstructorObjectLiteralSingleFunctionPropertyClass(bindToTarget.OnChange)
					),
					true
				);
			});
		}

		private void InstanceMethod(string value) { }

		private sealed class SingleStringPropertyClass
		{
			public SingleStringPropertyClass(string name)
			{
				Name = name;
			}
			public string Name { get; }
		}

		private sealed class SingleCaseInsensitiveStringPropertyClass
		{
			public SingleCaseInsensitiveStringPropertyClass(CaseInsensitiveString name)
			{
				Name = name;
			}
			public CaseInsensitiveString Name { get; }
		}

		private sealed class CaseInsensitiveString
		{
			public CaseInsensitiveString(string value)
			{
				Value = value;
			}
			public string Value { get; }
			public override bool Equals(object o)
			{
				var other = o as CaseInsensitiveString;
				return (other != null) && (other.Value ?? "").Equals(Value ?? "", StringComparison.OrdinalIgnoreCase);
			}
			public override int GetHashCode()
			{
				return (Value ?? "").ToUpper().GetHashCode();
			}
		}

		private sealed class SingleFunctionPropertyClass
		{
			public SingleFunctionPropertyClass(Action<string> onChange)
			{
				OnChange = onChange;
			}
			public Action<string> OnChange { get; }
		}

		private sealed class SingleActionPropertyClass
		{
			public SingleActionPropertyClass(Action work)
			{
				Work = work;
			}
			public Action Work { get; }
		}

		[ObjectLiteral(ObjectCreateMode.Plain)]
		private sealed class ObjectLiteralSingleStringPropertyClass
		{
			public string Name { get; set; }
		}

		[ObjectLiteral(ObjectCreateMode.Plain)]
		private sealed class ObjectLiteralSingleFunctionPropertyClass
		{
			public Action<string> OnChange { get; set; }
		}

		[ObjectLiteral(ObjectCreateMode.Plain)]
		private sealed class ObjectLiteralSingleActionPropertyClass
		{
			public Action Work { get; set; }
		}

		[ObjectLiteral(ObjectCreateMode.Constructor)]
		private sealed class ConstructorObjectLiteralSingleStringPropertyClass
		{
			public ConstructorObjectLiteralSingleStringPropertyClass(string name)
			{
				Name = name;
			}
			public string Name { get; }
		}

		[ObjectLiteral(ObjectCreateMode.Constructor)]
		private sealed class ConstructorObjectLiteralSingleFunctionPropertyClass
		{
			public ConstructorObjectLiteralSingleFunctionPropertyClass(Action<string> onChange)
			{
				OnChange = onChange;
			}
			public Action<string> OnChange { get; }
		}

		[ObjectLiteral(ObjectCreateMode.Constructor)]
		private sealed class ConstructorObjectLiteralSingleActionPropertyClass
		{
			public ConstructorObjectLiteralSingleActionPropertyClass(Action<string> onChange)
			{
				OnChange = onChange;
			}
			public Action<string> OnChange { get; }
		}
	}
}
