# Version 2 Breaking Changes

Version 2 marks a turning point for QueryFirst. We've taken the source private. We've got a load of new features, and some breaking changes, and we're going to start charging for a premium version. After 6 years as an open source project, the goal is to try something else to build momentum around the project.

## Breaking Changes

- No more user partial class. The template no longer includes this file, but the generated repo and poco are still marked as partial. The namespace is no longer read from this file. The default namespace, based on folder structure, is used. If you need to override this, you can add a "namespace" property to the query's config.
- The generated file renamed. .net core nests files based on filename, so, to enable nesting without changing settings, generated files are now called [MyQuery].sql.cs.
- The generated classes renamed. QueryFirst repositories and POCOs share a distinctive structure and role, so we wanted to put their provenance right up-front in the class names. 
  - Repositories are now called [MyQuery]QfRepo. Correspondingly, the interfaces are now I[MyQuery]QfRepo.
  - The results class is now [MyQuery]QfDto.
  - Migrating a project from QueryFirst 1.x to 2, this will be the most time-consuming change to implement. The best method we've found is to use visual studio to rename the generated classes before regenerating. This way, references are also renamed.
- We now use dependency injection for the connection factory. Repository instances using a static connection class was fence-sitting. Now we ask you to choose between full DI, with the connection factory injected into repositories, or full static. The static methods on the repository access the static connection factory.