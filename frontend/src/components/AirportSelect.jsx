import { useState, useEffect, useRef } from 'react';
import airportService from '../services/airportService';
import './AirportSelect.css';

const AirportSelect = ({ label, value, onChange, placeholder }) => {
  const [airports, setAirports] = useState([]);
  const [filteredAirports, setFilteredAirports] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [isDropdownOpen, setIsDropdownOpen] = useState(false);
  const [inputValue, setInputValue] = value || '';
  const dropdownRef = useRef(null);
  const inputRef = useRef(null);

  useEffect(() => {
    loadAirports();
  }, []);

  useEffect(() => {
    setInputValue(value || '');
  }, [value]);

  useEffect(() => {
    if (inputValue.length >= 1) {
      const filtered = airports.filter(airport =>
        airport.name.toLowerCase().includes(inputValue.toLowerCase()) ||
        airport.location.toLowerCase().includes(inputValue.toLowerCase()) ||
        airport.code?.toLowerCase().includes(inputValue.toLowerCase())
      );
      setFilteredAirports(filtered);
    } else {
      setFilteredAirports([]);
    }
  }, [inputValue, airports]);

  useEffect(() => {
    const handleClickOutside = (event) => {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target)) {
        setIsDropdownOpen(false);
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const loadAirports = async () => {
    try {
      setIsLoading(true);
      const data = await airportService.getAll();
      setAirports(data);
    } catch (error) {
      console.error('Failed to load airports:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleInputChange = (e) => {
    const newValue = e.target.value;
    setInputValue(newValue);
    setIsDropdownOpen(true);
    if (!newValue) {
      onChange('');
    }
  };

  const handleInputFocus = () => {
    setIsDropdownOpen(true);
  };

  const handleAirportSelect = (airport) => {
    setInputValue(`${airport.name} (${airport.location})`);
    onChange(airport);
    setIsDropdownOpen(false);
  };

  const handleClear = () => {
    setInputValue('');
    onChange('');
    inputRef.current?.focus();
  };

  return (
    <div className="airport-select-container" ref={dropdownRef}>
      <label className="airport-select-label">{label}</label>
      <div className="airport-select-wrapper">
        <input
          ref={inputRef}
          type="text"
          className="airport-select-input"
          placeholder={placeholder}
          value={inputValue}
          onChange={handleInputChange}
          onFocus={handleInputFocus}
          readOnly={false}
        />
        {inputValue && (
          <button className="airport-select-clear" onClick={handleClear}>
            ×
          </button>
        )}
        {isLoading && <span className="airport-select-loading">⏳</span>}
        <span className="airport-select-arrow">{isDropdownOpen ? '↑' : '↓'}</span>
      </div>
      
      {isDropdownOpen && filteredAirports.length > 0 && (
        <div className="airport-select-dropdown">
          {filteredAirports.map(airport => (
            <div
              key={airport.id}
              className="airport-select-option"
              onClick={() => handleAirportSelect(airport)}
            >
              <div className="airport-option-main">
                <span className="airport-code">{airport.code}</span>
                <span className="airport-name">{airport.name}</span>
              </div>
              <span className="airport-location">{airport.location}</span>
            </div>
          ))}
        </div>
      )}
      
      {isDropdownOpen && inputValue && filteredAirports.length === 0 && !isLoading && (
        <div className="airport-select-dropdown">
          <div className="airport-select-no-results">
            Аэропорты не найдены
          </div>
        </div>
      )}
    </div>
  );
};

export default AirportSelect;
