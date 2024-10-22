act -W ".github/workflows/tf-plan-apply.yml" \
-P ubuntu-18.04=catthehacker/ubuntu:full-latest \
--pull=false \
-s GITHUB_TOKEN="$(gh auth token)" \
--secret-file my.secrets \
--artifact-server-path act-cli-temp-artifacts