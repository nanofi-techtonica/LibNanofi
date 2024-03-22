# LibNanofi


## Contribute

### Build

To build the project, you need to create a configuration file `build.json` in the root directory. The file should look like this:

```json
{
  ...
  "GameRoot": "path/to/your/game/root",
  "BepinExRoot": "path/to/your/bepinex/root",
  ...
}
```

### Release

The release process is automated by the custom MSBuild task. You need to configure the `build.json` file to include the following fields:

```json
{
  ...
  "GithubRepo": "user/repo",
  "GithubToken": "TOKEN",
  ...
}
```

The release process can be started by publishing the `LibNanofi` project using `FolderProfile.pubxml` profile. The release process will create a new release on GitHub and upload the artifacts to the release.