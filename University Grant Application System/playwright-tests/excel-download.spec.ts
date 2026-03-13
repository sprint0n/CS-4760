import { test, expect } from '@playwright/test';
import fs from 'fs';

test('test', async ({ page }) => {
  await page.goto('https://localhost:7247/');
  await page.getByRole('textbox', { name: 'Email Address' }).click();
  await page.getByRole('textbox', { name: 'Email Address' }).fill('CoChair@mail.com');
  await page.getByRole('textbox', { name: 'Password' }).click();
  await page.getByRole('textbox', { name: 'Password' }).fill('123456');
  await page.getByRole('button', { name: 'Sign In' }).click();
  await expect(page.locator('#navbarContent')).toContainText('Allocation');
  await page.getByRole('link', { name: 'Allocation' }).click();
  await expect(page.getByRole('paragraph')).toContainText('Distribute Funding to Approved Applications');
  await page.waitForTimeout(500); // give JS time to run
  await page.getByRole('button', { name: 'Set Funding Criteria' }).click();

  await page.waitForTimeout(500); // give JS time to run

  console.log('Modal display:', await page.locator('#criteriaModal')
    .evaluate(el => getComputedStyle(el).display));


  // wait for modal to be visible before continuing
  await page.locator('#modalFullMin').waitFor({ state: 'visible' });

  await expect(page.locator('.modal-header')).toBeVisible();
  await page.getByPlaceholder('Min').click();
  await page.getByPlaceholder('Min').fill('70');
  await page.getByPlaceholder('Max').click();
  await page.getByPlaceholder('Max').fill('80');
  await page.getByPlaceholder('Alloc %').click();
  await page.getByPlaceholder('Alloc %').fill('80');
  await page.getByText('Add', { exact: true }).click();
  await page.getByPlaceholder('Min').click();
  await page.getByPlaceholder('Min').fill('80');
  await page.getByPlaceholder('Max').click();
  await page.getByPlaceholder('Max').fill('90');
  await page.getByPlaceholder('Alloc %').click();
  await page.getByPlaceholder('Alloc %').fill('90');
  await page.getByText('Add').click();
  await page.getByText('Apply Funds').click();
  await expect(page.getByRole('button', { name: 'Download Funded Grants Excel' })).toBeVisible();
  const downloadPromise = page.waitForEvent('download');
  await page.getByRole('button', { name: 'Download Funded Grants Excel' }).click();
  const download = await downloadPromise;

  // get the download file path
  const filePath = await download.path();
  expect(filePath).not.toBeNull();

  // verify the file is not empty
  const stats = fs.statSync(filePath!);
  expect(stats.size).toBeGreaterThan(0);

  // Log file details for debugging
  console.log("Downloaded file path:", filePath);
  console.log("Downloaded file size:", stats.size);
  console.log("Suggested filename:", download.suggestedFilename());

  console.log('Test completed successfully.');
});