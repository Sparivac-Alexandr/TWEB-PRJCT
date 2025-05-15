document.addEventListener('DOMContentLoaded', function() {
    // Edit section functionality
    const editButtons = document.querySelectorAll('.edit-btn');
    const editModal = document.getElementById('edit-modal');
    const closeModalBtn = document.getElementById('close-modal');
    const cancelEditBtn = document.getElementById('cancel-edit');
    const saveEditBtn = document.getElementById('save-edit');
    const modalTitle = document.getElementById('modal-title');
    const modalBody = document.getElementById('modal-body');
    
    let currentSection = '';
    let originalContent = '';
    
    editButtons.forEach(button => {
        button.addEventListener('click', function() {
            const section = this.getAttribute('data-section');
            currentSection = section;
            
            // Set modal title based on section
            switch(section) {
                case 'about':
                    modalTitle.textContent = 'Edit About Me';
                    originalContent = document.getElementById('about-content').innerHTML;
                    modalBody.innerHTML = `
                        <textarea id="edit-about" class="edit-textarea" rows="6">${document.getElementById('about-content').innerText.trim()}</textarea>
                    `;
                    break;
                case 'skills':
                    const userType = document.body.getAttribute('data-user-type') || 'freelancer';
                    modalTitle.textContent = (userType === 'freelancer') ? 'Edit My Skills' : 'Edit Skills I\'m Looking For';
                    originalContent = document.getElementById('skills-content').innerHTML;
                    
                    // Get existing skills as comma-separated string
                    const skillTags = document.querySelectorAll('.skill-tag');
                    const skillsText = Array.from(skillTags).map(tag => tag.textContent).join(', ');
                    
                    modalBody.innerHTML = `
                        <p>Enter your skills separated by commas</p>
                        <textarea id="edit-skills" class="edit-textarea" rows="4">${skillsText}</textarea>
                    `;
                    break;
                case 'preferences':
                    modalTitle.textContent = 'Edit My Preferences';
                    originalContent = document.getElementById('preferences-content').innerHTML;
                    
                    // Create form with existing preference values
                    const preferenceItems = document.querySelectorAll('.preference-item');
                    let preferencesForm = '<div class="edit-preferences">';
                    
                    preferenceItems.forEach(item => {
                        const label = item.querySelector('.preference-label').textContent.replace(':', '');
                        const value = item.querySelector('.preference-value').textContent;
                        
                        preferencesForm += `
                            <div class="preference-edit-item">
                                <label>${label}:</label>
                                <input type="text" value="${value}" class="preference-input">
                            </div>
                        `;
                    });
                    
                    preferencesForm += '</div>';
                    modalBody.innerHTML = preferencesForm;
                    break;
                case 'contact':
                    modalTitle.textContent = 'Edit Contact Information';
                    const contactItems = document.querySelectorAll('.contact-item');
                    let contactForm = '<div class="edit-contact">';
                    
                    contactItems.forEach(item => {
                        const icon = item.querySelector('i').className;
                        const value = item.querySelector('span').textContent;
                        let label = '';
                        
                        if (icon.includes('envelope')) label = 'Email';
                        else if (icon.includes('phone')) label = 'Phone';
                        else if (icon.includes('map-marker')) label = 'Location';
                        else if (icon.includes('globe')) label = 'Website';
                        
                        contactForm += `
                            <div class="contact-edit-item">
                                <label>${label}:</label>
                                <input type="text" value="${value}" class="contact-input">
                            </div>
                        `;
                    });
                    
                    contactForm += '</div>';
                    modalBody.innerHTML = contactForm;
                    break;
                case 'availability':
                    const userTypeForAvailability = document.body.getAttribute('data-user-type') || 'freelancer';
                    if (userTypeForAvailability === 'freelancer') {
                        modalTitle.textContent = 'Edit Availability';
                        
                        const status = document.querySelector('.availability-status').textContent.trim();
                        const hours = document.querySelector('.availability-hours span').textContent;
                        
                        modalBody.innerHTML = `
                            <div class="edit-availability">
                                <div class="availability-edit-item">
                                    <label>Status:</label>
                                    <select id="availability-status">
                                        <option value="available" ${status.includes('Available') ? 'selected' : ''}>Available for work</option>
                                        <option value="limited" ${status.includes('Limited') ? 'selected' : ''}>Limited availability</option>
                                        <option value="unavailable" ${status.includes('Unavailable') ? 'selected' : ''}>Not available</option>
                                    </select>
                                </div>
                                <div class="availability-edit-item">
                                    <label>Hours per week:</label>
                                    <input type="number" min="1" max="60" value="${hours.split(' ')[0]}" id="availability-hours">
                                </div>
                            </div>
                        `;
                    } else {
                        modalTitle.textContent = 'Edit Budget Range';
                        
                        const budgetRange = document.querySelector('.budget-range .budget-value').textContent;
                        const paymentType = document.querySelector('.budget-preference .budget-value').textContent;
                        
                        modalBody.innerHTML = `
                            <div class="edit-budget">
                                <div class="budget-edit-item">
                                    <label>Typical Budget:</label>
                                    <input type="text" value="${budgetRange}" id="budget-range">
                                </div>
                                <div class="budget-edit-item">
                                    <label>Preferred Payment:</label>
                                    <select id="payment-type">
                                        <option value="fixed" ${paymentType.includes('Fixed') ? 'selected' : ''}>Fixed Price</option>
                                        <option value="hourly" ${paymentType.includes('Hourly') ? 'selected' : ''}>Hourly Rate</option>
                                        <option value="milestone" ${paymentType.includes('Milestone') ? 'selected' : ''}>Milestone Based</option>
                                    </select>
                                </div>
                            </div>
                        `;
                    }
                    break;
            }
            
            // Show the modal
            editModal.classList.add('show');
        });
    });
    
    // Close modal events
    closeModalBtn.addEventListener('click', closeModal);
    cancelEditBtn.addEventListener('click', closeModal);
    
    function closeModal() {
        editModal.classList.remove('show');
    }
    
    // Save changes
    saveEditBtn.addEventListener('click', function() {
        // Different save logic based on the section
        switch(currentSection) {
            case 'about':
                const aboutText = document.getElementById('edit-about').value;
                document.getElementById('about-content').innerHTML = `<p>${aboutText.replace(/\n/g, '</p><p>')}</p>`;
                break;
            case 'skills':
                const skillsText = document.getElementById('edit-skills').value;
                const skills = skillsText.split(',').map(skill => skill.trim()).filter(skill => skill);
                
                let skillsHtml = '<div class="skills-container">';
                skills.forEach(skill => {
                    skillsHtml += `<span class="skill-tag">${skill}</span>`;
                });
                skillsHtml += '</div>';
                
                document.getElementById('skills-content').innerHTML = skillsHtml;
                break;
            case 'preferences':
                const preferenceInputs = document.querySelectorAll('.preference-input');
                const preferenceItems = document.querySelectorAll('.preference-item');
                
                preferenceInputs.forEach((input, index) => {
                    if (preferenceItems[index]) {
                        preferenceItems[index].querySelector('.preference-value').textContent = input.value;
                    }
                });
                break;
            case 'contact':
                const contactInputs = document.querySelectorAll('.contact-input');
                const contactItems = document.querySelectorAll('.contact-item span');
                
                contactInputs.forEach((input, index) => {
                    if (contactItems[index]) {
                        contactItems[index].textContent = input.value;
                    }
                });
                break;
            case 'availability':
                const userType = document.body.getAttribute('data-user-type') || 'freelancer';
                if (userType === 'freelancer') {
                    const status = document.getElementById('availability-status').value;
                    const hours = document.getElementById('availability-hours').value;
                    
                    const statusElement = document.querySelector('.availability-status');
                    const hoursElement = document.querySelector('.availability-hours span');
                    
                    // Update status text and class
                    statusElement.className = 'availability-status ' + status;
                    
                    if (status === 'available') {
                        statusElement.textContent = 'Available for work';
                    } else if (status === 'limited') {
                        statusElement.textContent = 'Limited availability';
                    } else {
                        statusElement.textContent = 'Not available';
                    }
                    
                    hoursElement.textContent = `${hours} hrs/week`;
                } else {
                    const budgetRange = document.getElementById('budget-range').value;
                    const paymentType = document.getElementById('payment-type').value;
                    
                    const rangeElement = document.querySelector('.budget-range .budget-value');
                    const typeElement = document.querySelector('.budget-preference .budget-value');
                    
                    rangeElement.textContent = budgetRange;
                    
                    if (paymentType === 'fixed') {
                        typeElement.textContent = 'Fixed Price';
                    } else if (paymentType === 'hourly') {
                        typeElement.textContent = 'Hourly Rate';
                    } else {
                        typeElement.textContent = 'Milestone Based';
                    }
                }
                break;
        }
        
        // Show success notification
        showNotification('Changes saved successfully!', 'success');
        
        // In a real app, you would save to the server here
        // simulate a server call
        console.log('Saving changes for section:', currentSection);
        
        // Close the modal
        closeModal();
    });
    
    // Notification function
    function showNotification(message, type) {
        const notification = document.createElement('div');
        notification.className = `notification ${type}`;
        notification.textContent = message;
        
        document.body.appendChild(notification);
        
        setTimeout(() => {
            notification.classList.add('show');
            
            setTimeout(() => {
                notification.classList.remove('show');
                setTimeout(() => {
                    notification.remove();
                }, 300);
            }, 3000);
        }, 10);
    }
}); 