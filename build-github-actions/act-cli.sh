docker build -t my-build-server:latest ./build-github-actions

act \
-W ".github/workflows/provision-deploy.yml" \
-s GITHUB_TOKEN="$(gh auth token)" \
-P ubuntu-latest=my-build-server:latest \
--secret-file my.secrets \
--artifact-server-path act-cli-temp-artifacts \
--var-file configurationVariables.secrets