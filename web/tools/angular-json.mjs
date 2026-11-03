// Docker'da ön yüzü Nx olmadan, doğrudan Angular CLI ile derlemek için angular.json üretir.
// Sebep: Nx'in kullandığı SQLite veritabanı eski çekirdekli sunucularda (ör. CentOS 7) "disk I/O error" veriyor.
// Derleme ayarları tek yerde kalsın diye apps/<ad>/project.json'daki build hedefinden okunur.
//
// Kullanım (web klasöründe): node tools/angular-json.mjs ui admin && npx ng build ui && npx ng build admin
import { readFileSync, writeFileSync } from 'node:fs';

const names = process.argv.slice(2);
if (names.length === 0) {
  console.error('Kullanım: node tools/angular-json.mjs <uygulama> [<uygulama>...]');
  process.exit(1);
}

const projects = {};
for (const name of names) {
  const project = JSON.parse(readFileSync(`apps/${name}/project.json`, 'utf8'));
  const { executor, options, configurations, defaultConfiguration } = project.targets.build;

  projects[name] = {
    root: `apps/${name}`,
    sourceRoot: project.sourceRoot,
    projectType: project.projectType,
    prefix: project.prefix,
    architect: {
      build: { builder: executor, options, configurations, defaultConfiguration },
    },
  };
}

const workspace = {
  version: 1,
  // Angular'ın disk önbelleği de yerel bir veritabanı (LMDB) kullanıyor; tek seferlik derlemede gerekmez.
  cli: { analytics: false, cache: { enabled: false } },
  projects,
};

writeFileSync('angular.json', JSON.stringify(workspace, null, 2));
console.log(`angular.json oluşturuldu: ${names.join(', ')}`);
