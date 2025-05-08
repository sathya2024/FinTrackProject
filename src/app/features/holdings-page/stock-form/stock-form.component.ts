import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { stockSearchComponent } from '../stock-search/stock-search.component';

@Component({
  selector: 'app-stock-form',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    stockSearchComponent,
  ],
  templateUrl: './stock-form.component.html',
  styleUrl: './stock-form.component.css',
})
export class stockFormComponent implements OnInit {
  @Input() data: any;
  @Input() isEditMode = false;
  @Output() save = new EventEmitter<any>();
  @Output() cancel = new EventEmitter<void>();

  form!: FormGroup;
  selectedInvestment: { transactionType: string } = { transactionType: '' };
  constructor(private fb: FormBuilder) {}

  ngOnInit() {
    this.form = this.fb.group({
      transactionType: [
        this.data?.transactionType || 'Buy' || 'Sell',
        Validators.required,
      ],
      stockName: [this.data?.stockName || '', Validators.required],
      dematAccount: [this.data?.dematAccount || '', Validators.required],
      purchaseDate: [this.data?.purchaseDate || '', Validators.required],
      numberOfShares: [this.data?.numberOfShares || '', Validators.required],
      brokerage: [this.data?.brokerage || '', Validators.required],
      brokerageType: [this.data?.brokerageType || '%', Validators.required],
      purchasePrice: [this.data?.purchasePrice || '', Validators.required],
    });
    if (this.isEditMode) {
      this.form.get('stockName')?.disable();
    }
  }

  onstockSelected(symbol: string) {
    if (!this.isEditMode) {
      this.form.get('stockName')?.setValue(symbol);
    }
  }

  submit() {
    if (this.form.valid) {
      this.save.emit(this.form.getRawValue());
    }
  }
}

