docker build -t my-build-server:latest ./build-github-actions

# Act CLI docs: https://nektosact.com/usage/index.html
clear | act -W ".github/workflows/CD.yml" -s GITHUB_TOKEN="$(gh auth token)" -P ubuntu-latest=my-build-server:latest --secret-file my.secrets --artifact-server-path act-cli-temp-artifacts --var-file configurationVariables.secrets --pull=false -e build-github-actions/act-event-payload.json # -j 'provision' 