import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { FormatterUtils } from '../../../utils/formatters';
import { EditButton } from '../../layout/edit-button/edit-button';
import { DeleteButton } from '../../layout/delete-button/delete-button';
import { CurrencyService } from '../../../services/gameplay/currency/currency-service';
import { Header } from '../../layout/header/header';

@Component({
  selector: 'app-currency',
  imports: [Header, CommonModule, ReactiveFormsModule, EditButton, DeleteButton],
  templateUrl: './currency.html',
  styleUrl: './currency.scss',
})
export class Currency {
currencyForm!: FormGroup;
  currencies = signal<any[]>([]);

  constructor(private fb: FormBuilder, private service: CurrencyService) {}

  ngOnInit(): void {
    this.currencyForm = this.fb.group({
      name: ['', [Validators.required]],
      rawValues: ['', [Validators.required]] 
    });
    this.loadCurrencies();
  }

  onSubmit() {
  if (this.currencyForm.valid) {
    const raw = this.currencyForm.value;
    
    const jsonString = FormatterUtils.parseInputToJson(raw.rawValues);
    const valuesObj = JSON.parse(jsonString);
    
    const payload = {
      name: raw.name,
      values: Object.entries(valuesObj).map(([key, val]) => ({
        name: key,
        conversionRate: parseFloat(val as string) 
      }))
    };

    console.log("Payload to send:", payload);

    this.service.create(payload).subscribe({
      next: (res) => {
        this.currencies.update(list => [res, ...list]);
        this.currencyForm.reset();
      },
      error: (err) => console.error("Server error:", err)
    });
  }
}

  onDelete(currency: any) {
    if (confirm(`Are you sure you want to delete the economy "${currency.name}"?`)) {
      this.service.delete(currency.id).subscribe({
        next: () => {
          this.currencies.update(list => list.filter(c => c.id !== currency.id));
        },
        error: (err) => console.error('Error deleting currency:', err)
      });
    }
  }

  loadCurrencies() {
    this.service.getCurrencies().subscribe(data => this.currencies.set(data));
  }
}
