#!/usr/bin/env bash

version=$(grep "<Version>" < src/${INTERNAL_NAME}/${INTERNAL_NAME}.csproj | sed "s/.*<Version>\(.*\)<\/Version>/\1/")
echo "> Version detected: ${version}"

echo "> Logging into GitHub"
echo "$1" | gh auth login --with-token
authorName=$(jq -r '.commits[0].author.name' "${GITHUB_EVENT_PATH}")
authorEmail=$(jq -r '.commits[0].author.email' "${GITHUB_EVENT_PATH}")
git config --global user.name "${authorName}"
git config --global user.email "${authorEmail}"

echo "> Getting dalamud plugins repo"
gh repo clone "${GITHUB_REPOSITORY_OWNER}/DalamudPluginsD17" repo
cd repo
git remote add pr_repo "https://github.com/goatcorp/DalamudPluginsD17.git"
git fetch pr_repo
git fetch origin
originUrl=$(git config --get remote.origin.url | cut -d '/' -f 3-)
originUrl="https://$1@${originUrl}"
git config remote.origin.url "${originUrl}"
branch="${PUBLIC_NAME}"
if git show-ref --quiet "refs/heads/${branch}"; then
    echo "> Branch ${branch} already exists, resetting to master"
    git checkout "${branch}"
    git reset --hard "pr_repo/main"
else
    echo "> Creating new branch ${branch}"
    git reset --hard "pr_repo/main"
    git branch "${branch}"
    git checkout "${branch}"
    git push --set-upstream origin --force "${branch}"
fi
cd ..

echo "> Deleting old plugin manifest files"
rm -rf repo/stable/${INTERNAL_NAME}
rm -rf repo/testing/live/${INTERNAL_NAME}
rm -rf repo/testing/net6/${INTERNAL_NAME}

echo "> Creating new plugin manifest directory"
commitMessage="${PUBLIC_NAME} ${version}"
if [[ ${MESSAGE} =~ .*"[TEST]".* ]]; then
    mkdir repo/testing/live/${INTERNAL_NAME}
    cp -r images repo/testing/live/${INTERNAL_NAME}
    cd repo/testing/live/${INTERNAL_NAME}
    commitMessage="[Testing] ${commitMessage}"
else
    mkdir repo/stable/${INTERNAL_NAME}
    cp -r images repo/stable/${INTERNAL_NAME}
    cd repo/stable/${INTERNAL_NAME}
fi

echo "> Creating new plugin toml manifest"
echo "[plugin]" >>manifest.toml
echo "repository = \"${GITHUB_SERVER_URL}/${GITHUB_REPOSITORY}.git\"" >>manifest.toml
echo "owners = [ \"${GITHUB_REPOSITORY_OWNER}\" ]" >>manifest.toml
echo "project_path = \"src/${INTERNAL_NAME}\"" >>manifest.toml
echo "commit = \"${GITHUB_SHA}\"" >>manifest.toml
cat manifest.toml

echo "> Adding and committing"
git add --all
git commit --all -m "${commitMessage}"

echo "> Pushing to origin"
git push --force --set-upstream origin "${PUBLIC_NAME}"

echo "> Done"