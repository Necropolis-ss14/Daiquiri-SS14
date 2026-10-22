p = r'D:\git\LightLine\Content.Client\_Daiquiri\CartridgeLoader\Cartridges\GoogolplexUiFragment.xaml'
s = open(p, encoding='utf-8').read()

# Замена ProgressBackground + SearchProgress в StarWayPage на уникальные имена
old = '''<TextureRect Name="ProgressBackground" HorizontalAlignment="Center" MinWidth="300" MinHeight="16" MaxHeight="16" Stretch="KeepAspectCentered" Margin="0 24 0 0"/>
                <ProgressBar Name="SearchProgress" HorizontalExpand="True" MinHeight="16" MaxHeight="16" Margin="36 8 36 8"/>
                <Button Name="StarWayBackButton"'''

new = '''<TextureRect Name="StarWayProgressBackground" HorizontalAlignment="Center" MinWidth="300" MinHeight="16" MaxHeight="16" Stretch="KeepAspectCentered" Margin="0 24 0 0"/>
                <ProgressBar Name="StarWaySearchProgress" HorizontalExpand="True" MinHeight="16" MaxHeight="16" Margin="36 8 36 8"/>
                <Button Name="StarWayBackButton"'''

s = s.replace(old, new)

open(p, 'w', encoding='utf-8').write(s)

print('ProgressBackground count:', s.count('ProgressBackground'))
print('StarWayProgressBackground count:', s.count('StarWayProgressBackground'))
print('SearchProgress count:', s.count('SearchProgress'))
print('StarWaySearchProgress count:', s.count('StarWaySearchProgress'))