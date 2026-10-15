import { chmod, copyFile, mkdir, readdir } from 'node:fs/promises';
import { dirname, join } from 'node:path';
import { fileURLToPath } from 'node:url';

const SCRIPTS_DIR = dirname(fileURLToPath(import.meta.url));
const SOURCE_DIR = join(SCRIPTS_DIR, 'git-hooks');
const TARGET_DIR = join(SCRIPTS_DIR, '../.git/hooks');

// Copia os hooks versionados em scripts/git-hooks/ para .git/hooks/.
async function installGitHooks() {
  const hooks = await readdir(SOURCE_DIR);

  await mkdir(TARGET_DIR, { recursive: true });

  for (const hook of hooks) {
    const target = join(TARGET_DIR, hook);
    await copyFile(join(SOURCE_DIR, hook), target);
    await chmod(target, 0o755);
    console.log(`✔ ${hook}`);
  }
}

installGitHooks();
